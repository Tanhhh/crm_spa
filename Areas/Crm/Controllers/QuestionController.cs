using System.Globalization;
using Erp.BackOffice.Crm.Models;
using Erp.BackOffice.Filters;
using Erp.Domain.Entities;
using Erp.Domain.Interfaces;
using Erp.Domain.Crm.Entities;
using Erp.Domain.Crm.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Erp.Utilities;
using WebMatrix.WebData;
using Erp.BackOffice.Helpers;
using Erp.BackOffice.Areas.Cms.Models;

namespace Erp.BackOffice.Crm.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    [Erp.BackOffice.Helpers.NoCacheHelper]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository QuestionRepository;
        private readonly IUserRepository userRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly ICategoryRepository _categoryRepository;
        public QuestionController(
            IQuestionRepository _Question
            , IUserRepository _user
            , IAnswerRepository _Answer
            , ICategoryRepository category
            )
        {
            QuestionRepository = _Question;
            userRepository = _user;
            answerRepository = _Answer;
            _categoryRepository = category;
        }

        #region Index

        public ViewResult Index(string txtSearch)
        {

            IQueryable<QuestionViewModel> q = QuestionRepository.GetAllQuestion()
                .Select(item => new QuestionViewModel
                {
                    Id = item.Id,
                    CreatedUserId = item.CreatedUserId,
                    //CreatedUserName = item.CreatedUserName,
                    CreatedDate = item.CreatedDate,
                    ModifiedUserId = item.ModifiedUserId,
                    //ModifiedUserName = item.ModifiedUserName,
                    ModifiedDate = item.ModifiedDate,
                    Name = item.Name,
                }).OrderByDescending(m => m.ModifiedDate);

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.FailedMessage = TempData["FailedMessage"];
            ViewBag.AlertMessage = TempData["AlertMessage"];
            return View(q);
        }
        #endregion

        #region Create
        public ViewResult Create()
        {
            var model = new QuestionViewModel();
            model.IsActivated = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var question = new Question();
                AutoMapper.Mapper.Map(model, question);
                question.IsDeleted = false;
                question.CreatedUserId = WebSecurity.CurrentUserId;
                question.ModifiedUserId = WebSecurity.CurrentUserId;
                question.AssignedUserId = WebSecurity.CurrentUserId;
                question.CreatedDate = DateTime.Now;
                question.ModifiedDate = DateTime.Now;
                QuestionRepository.InsertQuestion(question);

                //Thêm đáp án
                if (model.DetailList != null && model.DetailList.Count > 0)
                {
                    foreach (var item in model.DetailList)
                    {
                        var answer = new Answer();
                        AutoMapper.Mapper.Map(item, answer);
                        answer.IsDeleted = false;
                        answer.CreatedUserId = WebSecurity.CurrentUserId;
                        answer.ModifiedUserId = WebSecurity.CurrentUserId;
                        answer.AssignedUserId = WebSecurity.CurrentUserId;
                        answer.CreatedDate = DateTime.Now;
                        answer.ModifiedDate = DateTime.Now;

                        answer.QuestionId = question.Id;
                        answerRepository.InsertAnswer(answer);
                    }
                }

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion

        #region Edit
        public ActionResult Edit(int? Id)
        {
            var Question = QuestionRepository.GetQuestionById(Id.Value);
            if (Question != null && Question.IsDeleted != true)
            {
                var model = new QuestionViewModel();
                AutoMapper.Mapper.Map(Question, model);

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

                //Lấy danh sách đáp án
                var listAnswer = answerRepository.GetAllAnswer()
                    .Where(item => item.QuestionId == Question.Id).ToList();
                model.DetailList = new List<AnswerViewModel>();
                AutoMapper.Mapper.Map(listAnswer, model.DetailList);

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var Question = QuestionRepository.GetQuestionById(model.Id);
                    AutoMapper.Mapper.Map(model, Question);
                    Question.ModifiedUserId = WebSecurity.CurrentUserId;
                    Question.ModifiedDate = DateTime.Now;
                    QuestionRepository.UpdateQuestion(Question);

                    if (model.DetailList != null && model.DetailList.Count > 0)
                    {
                        //Thêm/Hoặc cập nhật đáp án
                        var listAnswerOld = answerRepository.GetAllAnswer()
                        .Where(item => item.QuestionId == Question.Id).ToList();

                        //Xóa những cái cũ
                        if (listAnswerOld != null && listAnswerOld.Count > 0)
                        {
                            foreach (var item in listAnswerOld)
                            {
                                var answer = model.DetailList.Where(i => i.Id == item.Id).FirstOrDefault();
                                if (answer == null)
                                {
                                    answerRepository.DeleteAnswer(item.Id);
                                }
                            }
                        }

                        //Thêm/cập nhật
                        foreach (var item in model.DetailList)
                        {
                            var answer = listAnswerOld.Where(i => i.Id == item.Id).FirstOrDefault();
                            if (answer == null)
                            {
                                answer = new Answer();
                                AutoMapper.Mapper.Map(item, answer);
                                answer.IsDeleted = false;
                                answer.CreatedUserId = WebSecurity.CurrentUserId;
                                answer.ModifiedUserId = WebSecurity.CurrentUserId;
                                answer.AssignedUserId = WebSecurity.CurrentUserId;
                                answer.CreatedDate = DateTime.Now;
                                answer.ModifiedDate = DateTime.Now;

                                answer.QuestionId = Question.Id;
                                answerRepository.InsertAnswer(answer);
                            }
                            else
                            {
                                answer.OrderNo = item.OrderNo;
                                answer.Content = item.Content;
                                answer.IsActivated = item.IsActivated;
                                answerRepository.UpdateAnswer(answer);
                            }
                        }
                    }

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("Index");
                }

                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion

        #region Detail
        public ActionResult Detail(int? Id)
        {
            var Question = QuestionRepository.GetQuestionById(Id.Value);
            if (Question != null && Question.IsDeleted != true)
            {
                var model = new QuestionViewModel();
                AutoMapper.Mapper.Map(Question, model);

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete()
        {
            try
            {
                string idDeleteAll = Request["DeleteId-checkbox"];
                string[] arrDeleteId = idDeleteAll.Split(',');
                for (int i = 0; i < arrDeleteId.Count(); i++)
                {
                    var item = QuestionRepository.GetQuestionById(int.Parse(arrDeleteId[i], CultureInfo.InvariantCulture));
                    if (item != null)
                    {
                        if (item.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                        {
                            TempData["FailedMessage"] = "NotOwner";
                            return RedirectToAction("Index");
                        }

                        item.IsDeleted = true;
                        QuestionRepository.UpdateQuestion(item);
                    }
                }
                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.DeleteSuccess;
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                TempData[Globals.FailedMessageKey] = App_GlobalResources.Error.RelationError;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region List Question
        List<CategoryViewModel> getCategories(string code)
        {
            List<CategoryViewModel> listCategory = new List<CategoryViewModel>();
            var model = _categoryRepository.GetAllCategories()
                .Where(item => item.Code == code)
                .OrderBy(m => m.OrderNo).ToList();
            if (model.Count() > 0)
            {
                AutoMapper.Mapper.Map(model, listCategory);
            }
            return listCategory;
        }

        public ViewResult List()
        {

            ViewBag.SuccessMessage = TempData[Globals.SuccessMessageKey];
            ViewBag.FailedMessage = TempData[Globals.FailedMessageKey];
            var model = new List<CategoryViewModel>();
            var code = "TemplateListVote";
            model = getCategories(code);
            var _list_question = QuestionRepository.GetAllQuestion()
                                 .Select(item1 => new QuestionViewModel
                                 {
                                     Id = item1.Id,
                                     Name = item1.Name,
                                     Type = item1.Type,
                                     Category = item1.Category,
                                     OrderNo = item1.OrderNo
                                 }).OrderByDescending(m => m.OrderNo);

            var list_question = new List<QuestionViewModel>();
            foreach (var item in model)
            {
                item.Level = 0;
                var aa = getCategories(item.Value);
                if (aa.Count() > 0)
                {
                    model=model.Union(aa).ToList();
                    foreach (var q in aa)
                    {
                        var _question=_list_question.Where(x=>x.Category==q.Value).ToList();
                        if (_question.Any())
                        {
                            list_question = list_question.Union(_question).ToList();
                        }
                        q.Level = 1;
                        var qq = getCategories(q.Value);
                                foreach (var q1 in qq)
                                {
                                    q1.Level = 2;
                                    var _question1 = _list_question.Where(x => x.Category == q1.Value).ToList();
                                    if (_question1.Any())
                                    {
                                        list_question = list_question.Union(_question1).ToList();
                                    }
                                }
                        model = model.Union(qq).ToList();
                    }
                }
            }
            ViewBag.list_question = list_question;

            return View(model);
        }

        #endregion

        #region CreateQ
        public ViewResult CreateQ(string category)
        {
            var model = new QuestionViewModel();
            model.IsActivated = true;
            model.Category = category;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateQ(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var type = Request["group_choice"];
                var question = new Question();
                AutoMapper.Mapper.Map(model, question);
                question.IsDeleted = false;
                question.CreatedUserId = WebSecurity.CurrentUserId;
                question.ModifiedUserId = WebSecurity.CurrentUserId;
                question.AssignedUserId = WebSecurity.CurrentUserId;
                question.CreatedDate = DateTime.Now;
                question.ModifiedDate = DateTime.Now;
                question.Type = type;
                QuestionRepository.InsertQuestion(question);

                if (type != "input")
                {
                    //Thêm đáp án
                    if (model.DetailList != null && model.DetailList.Count > 0)
                    {
                        foreach (var item in model.DetailList)
                        {
                            var answer = new Answer();
                            AutoMapper.Mapper.Map(item, answer);
                            answer.IsDeleted = false;
                            answer.CreatedUserId = WebSecurity.CurrentUserId;
                            answer.ModifiedUserId = WebSecurity.CurrentUserId;
                            answer.AssignedUserId = WebSecurity.CurrentUserId;
                            answer.CreatedDate = DateTime.Now;
                            answer.ModifiedDate = DateTime.Now;

                            answer.QuestionId = question.Id;
                            answerRepository.InsertAnswer(answer);
                        }
                    }
                }
                //ghi log 
                Erp.BackOffice.Controllers.HomeController.WriteLog(question.Id, model.Name, "đã tạo câu hỏi", "Question/List", Helpers.Common.CurrentUser.BranchId.Value);

                TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.InsertSuccess;
                return RedirectToAction("List");
            }
            return View(model);
        }
        #endregion

        #region EditQ
        public ActionResult EditQ(int? Id)
        {
            var Question = QuestionRepository.GetQuestionById(Id.Value);
            if (Question != null && Question.IsDeleted != true)
            {
                var model = new QuestionViewModel();
                AutoMapper.Mapper.Map(Question, model);

                if (model.CreatedUserId != Helpers.Common.CurrentUser.Id && Helpers.Common.CurrentUser.UserTypeId != 1)
                {
                    TempData["FailedMessage"] = "NotOwner";
                    return RedirectToAction("Index");
                }

                //Lấy danh sách đáp án
                var listAnswer = answerRepository.GetAllAnswer()
                    .Where(item => item.QuestionId == Question.Id).ToList();
                model.DetailList = new List<AnswerViewModel>();
                AutoMapper.Mapper.Map(listAnswer, model.DetailList);

                return View(model);
            }
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditQ(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request["Submit"] == "Save")
                {
                    var type = Request["group_choice"];
                    var Question = QuestionRepository.GetQuestionById(model.Id);
                    AutoMapper.Mapper.Map(model, Question);
                    Question.ModifiedUserId = WebSecurity.CurrentUserId;
                    Question.ModifiedDate = DateTime.Now;
                    Question.Type = type;
                    QuestionRepository.UpdateQuestion(Question);
                
                    if (model.DetailList != null && model.DetailList.Count > 0)
                    {
                        //Thêm/Hoặc cập nhật đáp án
                        var listAnswerOld = answerRepository.GetAllAnswer()
                        .Where(item => item.QuestionId == Question.Id).ToList();

                        //Xóa những cái cũ
                        if (listAnswerOld != null && listAnswerOld.Count > 0)
                        {
                            foreach (var item in listAnswerOld)
                            {
                                var answer = model.DetailList.Where(i => i.Id == item.Id).FirstOrDefault();
                                if (answer == null)
                                {
                                    answerRepository.DeleteAnswer(item.Id);
                                }
                            }
                        }
                        if (type != "input")
                        {
                            //Thêm/cập nhật
                            foreach (var item in model.DetailList)
                            {
                                var answer = listAnswerOld.Where(i => i.Id == item.Id).FirstOrDefault();
                                if (answer == null)
                                {
                                    answer = new Answer();
                                    AutoMapper.Mapper.Map(item, answer);
                                    answer.IsDeleted = false;
                                    answer.CreatedUserId = WebSecurity.CurrentUserId;
                                    answer.ModifiedUserId = WebSecurity.CurrentUserId;
                                    answer.AssignedUserId = WebSecurity.CurrentUserId;
                                    answer.CreatedDate = DateTime.Now;
                                    answer.ModifiedDate = DateTime.Now;

                                    answer.QuestionId = Question.Id;
                                    answerRepository.InsertAnswer(answer);
                                }
                                else
                                {
                                    answer.OrderNo = item.OrderNo;
                                    answer.Content = item.Content;
                                    answer.IsActivated = item.IsActivated;
                                    answerRepository.UpdateAnswer(answer);
                                }
                            }
                        }
                    }
                    //ghi log 
                    Erp.BackOffice.Controllers.HomeController.WriteLog(Question.Id, model.Name, "đã cập nhật câu hỏi", "Question/List", Helpers.Common.CurrentUser.BranchId.Value);

                    TempData[Globals.SuccessMessageKey] = App_GlobalResources.Wording.UpdateSuccess;
                    return RedirectToAction("List");
                }

                return View(model);
            }

            return View(model);

            //if (Request.UrlReferrer != null)
            //    return Redirect(Request.UrlReferrer.AbsoluteUri);
            //return RedirectToAction("Index");
        }

        #endregion
    }
}
