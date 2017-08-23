using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Controllers
{
    public class AnalyseController : Controller
    {
        // GET: Quest/Analyse
        public ActionResult Index()
        {
            // 1. Verbindung zur Datenbank
            var db = new QuestDbContext();

            // 2. Daten abfragen
            var model = db.Users.ToList();


            return View(model);
        }


        public ActionResult DownloadLogData()
        {
            var db = new QuestDbContext();

            // 2. Daten abfragen
            var model = db.Users.ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Datum;Geschlecht;Alter;Hochschulzugang;Fragen;Richtig;Falsch;NB;Zeit");
            writer.Write(Environment.NewLine);

            foreach (var user in model)
            {
                var startTime = user.Created;
                var lastTime = user.Created;

                var q = user.Logs.Count;
                var correct = 0;
                var wrong = 0;
                var na = 0;

                writer.Write("{0};{1};{2};{3}",
                    user.Created,
                    user.Geschlecht, user.Altersgruppe, user.Hochschulzugangsberechtigung);

                foreach (var log in user.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    if (log.Answer != null)
                    {
                        if (log.Answer.IsCorrect)
                        {
                            correct++;
                        }
                        else
                        {
                            wrong++;
                        }
                    }
                    else
                    {
                        na++;
                    }
                    if (log.FirstView > lastTime)
                        lastTime = log.FirstView;
                }

                writer.Write(";{0};{1};{2};{3}", q, correct, wrong, na);
                writer.Write(";{0}", (lastTime - startTime).TotalSeconds);

                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("WiQuestLog_");
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());

        }

        
        /*
         * Das sind die Details
        public ActionResult DownloadLogData()
        {
            var db = new QuestDbContext();

            // 2. Daten abfragen
            var model = db.Users.ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Datum;Geschlecht;Alter;Hochschulzugang");

            var nQuestions = 0;

            var firstUser = model.FirstOrDefault(x => x.Logs.Max(l => l.Answer != null));
            if (firstUser != null)
            {
                nQuestions = firstUser.Logs.Count;

                foreach (var log in firstUser.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    writer.Write(";F {0}_{1}", log.Question.Category.Reihenfolge, log.Question.Reihenfolge);
                }
                foreach (var log in firstUser.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    writer.Write(";T {0}_{1}", log.Question.Category.Reihenfolge, log.Question.Reihenfolge);
                }
                writer.Write(";Time");
            }

            writer.Write(Environment.NewLine);

            foreach (var user in model)
            {
                var startTime = user.Created;
                var lastTime = user.Created;


                writer.Write("{0};{1};{2};{3}",
                    user.Created,
                    user.Geschlecht, user.Altersgruppe, user.Hochschulzugangsberechtigung);

                var i = 0;
                foreach (var log in user.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    if (log.Answer != null)
                    {
                        // richtig = 1
                        // falsch = -1
                        writer.Write(";{0}", log.Answer.IsCorrect ? 1 : -1);
                    }
                    else
                    {
                        // unbeantwirtet = 0
                        writer.Write(";0");
                    }
                    i++;
                }
                for (var j = i; j < nQuestions; j++)
                {
                    writer.Write(";");
                }

                i = 0;
                foreach (var log in user.Logs.OrderBy(l => l.Question.Category.Reihenfolge).ThenBy(l => l.Question.Reihenfolge))
                {
                    if (log.FirstView > lastTime)
                        lastTime = log.FirstView;
                    writer.Write(";{0}", log.FirstView);
                    i++;
                }
                for (var j = i; j < nQuestions; j++)
                {
                    writer.Write(";");
                }

                writer.Write(";{0}",(lastTime-startTime).TotalSeconds);

                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("WiQuestLog_");
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
            
        }
         */
    }
}