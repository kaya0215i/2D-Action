using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MySqlConnector;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.ComponentModel.DataAnnotations;

namespace Function20241108
{
    public static class UserProvider
    {
        //DB接続情報
        static MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
        {
#if DEBUG
            Server = "localhost",
            Database = "2d_action_db",
            UserID = "root",
            Password = "",
            //SslMode = MySqlSslMode.Required,

#else
            Server = "db-ge-202409.mysql.database.azure.com",
            Database = "2d_action_db",
            UserID = "student",
            Password = "Yoshidajobi2024",
            SslMode = MySqlSslMode.Required,

#endif
        };

        [FunctionName("GetScoreRanking")]
        public static async Task<IActionResult> GetScoreRanking(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "scores/ranking/get")] HttpRequest req,
            ILogger log)
        {
            //DBサーバーに接続
            using var conn = new MySqlConnection(connectionStringBuilder.ConnectionString);
            await conn.OpenAsync();

            //string stageId = req.Query["stageId"]; //GETメソッドのパラメーターを取得

            string responseMessage;

            //SQLの作成
            MySqlCommand command = conn.CreateCommand();

            if (string.IsNullOrEmpty(req.Query["stageId"]))
            {
                //IDの指定がない場合は全員分のデータを取得
                command.CommandText = "select (select count(*) + 1 from highscores where score > high.score) as ranking, high.user_id, high.stage_id, high.score from highscores as high order by high.score desc;";

                //AQLクエリを発行し、結果を読み込む
                List<Ranking> rankingList = new List<Ranking>();
                using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    //取得レコード分繰り返し、リストに格納
                    Ranking ranking = new Ranking();
                    ranking.ranking = (long)reader[0];
                    ranking.userId = (int)reader[1];
                    ranking.stageId = (int)reader[2];
                    ranking.score = (int)reader[3];
                    rankingList.Add(ranking);
                }

                //userListをJsonシリアライズ
                responseMessage = JsonConvert.SerializeObject(rankingList);
            }
            else
            {
                //クエリパラメータの文字列を整数に変換
                bool isSuccess = int.TryParse(req.Query["stageId"], out int stageId);
                if (!isSuccess)
                {
                    //パラメータ指定異常時はBadRequestを返す
                    return new BadRequestResult(); //400エラー
                }

                //stageIDの指定がある場合は指定のデータのみ取得
                //SQL文に＠stageIdをいれ、AddWithValue関数で＠stageIdに値を割り入れる
                command.CommandText = "select (select count(*) + 1 from highscores where score > high.score and stage_id = @stageId) as ranking, high.user_id, user.name, high.stage_id, high.score from highscores as high left join user on user.id = high.user_id where stage_id = @stageId order by high.score desc limit 4;";
                command.Parameters.AddWithValue("@stageId", stageId);

                //AQLクエリを発行し、結果を読み込む
                List<Ranking> rankingList = new List<Ranking>();
                using MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    //取得レコード分繰り返し、リストに格納
                    Ranking ranking = new Ranking();
                    ranking.ranking = (long)reader[0];
                    ranking.userId = (int)reader[1];
                    ranking.userName = (string)reader[2];
                    ranking.stageId = (int)reader[3];
                    ranking.score = (int)reader[4];
                    rankingList.Add(ranking);
                }

                //rankingをJSONシリアライズ
                responseMessage = JsonConvert.SerializeObject(rankingList);
            }

            return new ContentResult()
            {
                StatusCode = 200,                 //ステータスコード
                ContentType = "application/json", //HTTPヘッダ
                Content = responseMessage         //レスポンスボディ
            };
        }

        [FunctionName("AddScore")]
        public static async Task<IActionResult> AddScore(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "scores/add")] HttpRequest req,
            ILogger log)
        {
            //DBサーバーに接続
            using var conn = new MySqlConnection(connectionStringBuilder.ConnectionString);
            await conn.OpenAsync();

            //JSONデシリアライズ
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                //リクエストボディが設定されていない
                return new BadRequestResult(); //400エラー
            }

            FindUser findUser;

            try
            {
                findUser = JsonConvert.DeserializeObject<FindUser>(requestBody);
                bool isSuccess = Validator.TryValidateObject(
                    findUser,
                    new ValidationContext(findUser, null, null),
                    null,
                    true
                );
                if(!isSuccess)
                {
                    return new BadRequestResult(); //400エラー
                }
            }
            catch (Newtonsoft.Json.JsonException e)
            {
                return new BadRequestObjectResult("不正な文字");　//400エラー
            }
            catch (Exception ex)
            {
                //Jsonのデシリアライズに失敗した
                return new BadRequestResult(); //400エラー
            }

            MySqlCommand command = conn.CreateCommand();

            command.CommandText = "insert into user(name) values (@userName) on duplicate key update name = values(name);";
            command.Parameters.AddWithValue("@userName", findUser.userName);
            await command.ExecuteNonQueryAsync();

            command.CommandText = "select id from user where name = @userName;";

            using MySqlDataReader reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            int userId = (int)reader[0];

            reader.Close();            

            command.CommandText = "insert into highscores(user_id, stage_id, score) VALUES (@userId, @stageId, @score) on duplicate key update score = if(score>values(score),score,values(score));";
            
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@stageId", findUser.stageId);
            command.Parameters.AddWithValue("@score", findUser.score);

            await command.ExecuteNonQueryAsync();
            return new OkObjectResult("Completed");
        }
    }
}
