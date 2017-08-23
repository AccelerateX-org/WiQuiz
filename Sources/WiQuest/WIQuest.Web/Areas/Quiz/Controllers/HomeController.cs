using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WIQuest.Web.Areas.Quiz.Models;
using WIQuest.Web.Controllers;
using WIQuest.Web.Data;
using WIQuest.Web.Models;
using WIQuest.Web.Services;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Areas.Quiz.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Quiz/Home
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var model = new PersonalGameList();

            var allPlans = QuizDb.TrainingPlans.ToList();

            foreach (var trainingPlan in allPlans)
            {
                if (trainingPlan.Groups.Any(x => x.Subscriptions.Any(y => y.Userid.Equals(user.Id))))
                {
                    model.MySubscribedPlans.Add(trainingPlan);
                }
                else
                {
                    model.AvailablePlans.Add(trainingPlan);
                }
            }


            foreach (var plan in model.MySubscribedPlans)
            {
                // Alle Gruppen in denen in drin bin
                var subscriptions = QuizDb.TrainingGroupSubscriptions.Where(x => 
                    x.Userid.Equals(user.Id) &&
                    x.TrainingGroup.TrainingPlan.Id == plan.Id
                    ).ToList();


                foreach (var session in plan.Sessions)
                {
                    foreach (var exercise in session.Exercises.Where(x => x.Quiz != null))
                    {
                        // keine Restriktion => für alle Gruppen aufnehmen
                        if (!exercise.Schedules.Any())
                        {
                            foreach (var subscription in subscriptions)
                            {
                                model.Exercises.Add(new PersonalTrainingExercise
                                {
                                    Exercise = exercise,
                                    Subscription = subscription
                                });
                            }
                        }
                        else
                        {
                            // Für jede Gruppe prüfen
                            foreach (var subscription in subscriptions)
                            {
                                var subscription1 = subscription;
                                // gibt es Zeitrestriktionen
                                var schedules =
                                    exercise.Schedules.Where(x => x.Group.Id == subscription1.TrainingGroup.Id);

                                if (schedules.Any())
                                {
                                    // ja, dann zeitprüfen
                                    foreach (var schedule in schedules)
                                    {
                                        // jetzt noch die Zeit prüfen
                                    }
                                }
                                else
                                {
                                    // nein, dann hinzufügen
                                    model.Exercises.Add(new PersonalTrainingExercise
                                    {
                                        Exercise = exercise,
                                        Subscription = subscription
                                    });
                                }
                            }
                        }
                    }
                }


            }


            
            
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">QuizId</param>
        /// <returns></returns>
        public ActionResult StartGame(Guid id)
        {
            // User identifizieren
            var user = GetCurrentUser();

            // Spiel anlegen
            var gameService = new GamePlayService();
            var game = gameService.CreateSingleUserGame(user.Id, id);

            var gameState = new GamePlayState();
            gameState.Game = game;
            gameState.Player = game.Players.First();
            gameState.Quiz = game.Levels.First().Quiz;

            // Den Fragenkatalog sortiert aufbauen
            foreach (var quizSection in gameState.Quiz.Sections.OrderBy(x => x.Order))
            {
                foreach (var quizQuestion in quizSection.Questions.OrderBy(x => x.Order))
                {
                    gameState.QuestionsLeft.Add(quizQuestion.Question);
                }
            }

            // TODO: Hier jetzt die Option des Quiz einbauen
            // Fragen mischen
            gameState.QuestionsLeft.Shuffle();


            // Statistik erstellen
            gameState.QuestionTotalCount = gameState.QuestionsLeft.Count;

            // In einer Session sichern
            // oder in DB?
            Session["GameState"] = gameState;


            var question = gameState.QuestionsLeft.FirstOrDefault();
            
            var model = new QuestionViewModel();
            model.Question = question;
            model.Answers = question.Answers.ToList();
            model.Answers.Shuffle();
            model.Total = gameState.QuestionTotalCount;
            model.Index = gameState.QuestionTotalCount - gameState.QuestionsLeft.Count + 1;

            gameState.CurrentQuestion = model;

            //return View("PlayGameSPA", model);
            return View("PlayGame", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exId">ExerciseId</param>
        /// <param name="subId">Subscription</param>
        /// <returns></returns>
        public ActionResult StartExercise(Guid exId, Guid subId)
        {
            // User identifizieren
            var user = GetCurrentUser();

            var exercise = QuizDb.TrainingExercises.SingleOrDefault(x => x.Id == exId);
            var subscription = QuizDb.TrainingGroupSubscriptions.SingleOrDefault(x => x.Id == subId);

            

            // Spiel anlegen
            var gameService = new GamePlayService(QuizDb);
            var game = gameService.CreateSingleUserGame(user.Id, exercise.Quiz.Id);

            var practice = new TrainingPractice
            {
                Exercise = exercise,
                Game = game,
                Subscription = subscription
            };
            QuizDb.TrainingPractices.Add(practice);
            QuizDb.SaveChanges();

            var gameState = new GamePlayState
            {
                Game = game,
                Player = game.Players.First(),
                Quiz = game.Levels.First().Quiz,
                Practice = practice
            };

            // Den Fragenkatalog sortiert aufbauen
            foreach (var quizSection in gameState.Quiz.Sections.OrderBy(x => x.Order))
            {
                foreach (var quizQuestion in quizSection.Questions.OrderBy(x => x.Order))
                {
                    gameState.QuestionsLeft.Add(quizQuestion.Question);
                }
            }

            // TODO: Hier jetzt die Option des Quiz einbauen
            // Fragen mischen
            gameState.QuestionsLeft.Shuffle();


            // Statistik erstellen
            gameState.QuestionTotalCount = gameState.QuestionsLeft.Count;

            // In einer Session sichern
            // oder in DB?
            Session["GameState"] = gameState;


            var question = gameState.QuestionsLeft.FirstOrDefault();

            var model = new QuestionViewModel
            {
                Question = question,
                Answers = question.Answers.ToList()
            };
            model.Answers.Shuffle();
            model.Total = gameState.QuestionTotalCount;
            model.Index = gameState.QuestionTotalCount - gameState.QuestionsLeft.Count + 1;

            gameState.CurrentQuestion = model;

            //return View("PlayGameSPA", model);
            return View("PlayGame", model);
        }


        [HttpPost]
        public PartialViewResult Answer(Guid questionId, Guid answerId)
        {
            var gameState = Session["GameState"] as GamePlayState;

            var answer = QuizDb.Answers.SingleOrDefault(x => x.Id == answerId);
            var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameState.Game.Id);
            var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == gameState.Player.Id);
            
            var log = new GameLog();
            log.Answer = answer;
            log.Game = game;
            log.Player = player;
            log.LogDateTime = DateTime.Now;

            QuizDb.GameLogs.Add(log);
            QuizDb.SaveChanges();

            // Update
            var statService = new StatisticsService(QuizDb);
            statService.NotifyUpdates();

            var questionToDelete = gameState.QuestionsLeft.SingleOrDefault(x => x.Id == answer.Question.Id);

            gameState.QuestionsLeft.Remove(questionToDelete);

            // jetzt die Antwort mit Ergebnis zurückschicken


            // die Frage
            // die Antwort
            var model = new QuestionAnswerViewModel();
            model.Answers = gameState.CurrentQuestion.Answers;
            model.GivenAnswer = answer;
            model.CorrectAnswer = questionToDelete.Answers.FirstOrDefault(x => x.IsCorrect);

            return PartialView("_Answer", model);
        }

        [HttpPost]
        public PartialViewResult NextQuestion()
        {
            var gameState = Session["GameState"] as GamePlayState;


            var question = gameState.QuestionsLeft.FirstOrDefault();

            if (question != null)
            {
                var model = new QuestionViewModel();
                model.Question = question;
                model.Answers = question.Answers.ToList();
                // TODO: Antworten nicht durchmischen
                model.Answers.Shuffle();
                model.Total = gameState.QuestionTotalCount;
                model.Index = gameState.QuestionTotalCount - gameState.QuestionsLeft.Count + 1;

                gameState.CurrentQuestion = model;

                return PartialView("_Question", model);
            }


            var game = QuizDb.QuizGames.SingleOrDefault(x => x.Id == gameState.Game.Id);
            var player = QuizDb.GamePlayers.SingleOrDefault(x => x.Id == gameState.Player.Id);

            var statisticsService = new GameStatisticsService(gameState.Quiz.Id, player.UserId);

            var statistics = statisticsService.GetStatistics(game.Id);

            return PartialView("_EndGame", statistics);
        }

        public ActionResult SubscribeGroup(Guid id)
        {

            var group = QuizDb.TrainingGroups.SingleOrDefault(x => x.Id == id);

            if (group == null)
                return RedirectToAction("Index");

            var user = GetCurrentUser();

            if (group.IsPublic)
            {
                var subscription = new TrainingGroupSubscription
                {
                    TrainingGroup = group,
                    TimeStamp = DateTime.Now,
                    Userid = user.Id
                };

                QuizDb.TrainingGroupSubscriptions.Add(subscription);
                QuizDb.SaveChanges();

                return RedirectToAction("Index");
            }

            var model = new TrainingGroup();

            return View(model);
        }

        [HttpPost]
        public ActionResult SubscribeGroup(TrainingGroup model)
        {
            var group = QuizDb.TrainingGroups.SingleOrDefault(x => x.Id == model.Id);

            if (group == null)
                return RedirectToAction("Index");

            var user = GetCurrentUser();

            if (group.AccessToken.Equals(model.AccessToken))
            {
                var subscription = new TrainingGroupSubscription
                {
                    TrainingGroup = group,
                    TimeStamp = DateTime.Now,
                    Userid = user.Id
                };

                QuizDb.TrainingGroupSubscriptions.Add(subscription);
                QuizDb.SaveChanges();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("AccessToken", "Falscher Schlüssel");


            return View(model);
        }


    }
}