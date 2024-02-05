

using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.DistanceLearning.Controllers {
    public static class QuizController {
        //
        //====================================================================================================
        /// <summary>
        /// Create a new quiz response. Call only if no response exists or the current one get close
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        public static QuizResponseModel createNewQuizResponse(CPBaseClass cp, QuizModel quiz) {
            //
            // -- get the current user
            var user = DbBaseModel.create<PersonModel>(cp, cp.User.Id);
            if (user == null) {
                throw new ApplicationException("The current user [" + cp.User.Id + "] is not valid.");
            }
            return createNewQuizResponse(cp, quiz, user);
        }
        //
        //====================================================================================================
        /// <summary>
        /// Create a new quiz response. Call only if no response exists or the current one get close
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        public static QuizResponseModel createNewQuizResponse(CPBaseClass cp, QuizModel quiz, PersonModel user) {
            DistanceLearning.Models.QuizResponseModel response = null/* TODO Change to default(_) if this is not a reference type */;
            try {
                List<QuizResponseModel> previousResponses = DistanceLearning.Models.QuizResponseModel.getResponseList(cp, quiz.id, user.id);
                // 
                // -- all previous responses must be complete
                if (previousResponses.Count > 0) {
                    foreach (var responsex in previousResponses) {
                        if ((!DistanceLearning.Controllers.GenericController.isDateEmpty(responsex.dateSubmitted))) {
                            responsex.dateSubmitted = DateTime.Now;
                            responsex.save(cp);
                        }
                    }
                }
                // 
                // -- housekeep subject FK
                cp.Db.ExecuteNonQuery("update quizquestions set subjectid=null where id in (select q.id from quizquestions q left join quizsubjects s on s.id=q.subjectid where s.id is null)");
                // 
                // -- add a new response, and create all the response details (with no answer selected)
                response = DbBaseModel.addDefault<QuizResponseModel>(cp);
                response.quizID = quiz.id;
                response.name = user.name + ", " + quiz.name + ", attempt:" + (previousResponses.Count + 1) + ", " + DateTime.Now.ToShortDateString();
                response.memberID = user.id;
                response.attemptNumber = previousResponses.Count + 1;
                response.save(cp);
                // 
                List<QuizSubjectModel> quizSubjectList = DbBaseModel.createList<QuizSubjectModel>(cp, "(quizId=" + quiz.id + ")");
                List<QuizQuestionModel> quizQuestionList = new List<QuizQuestionModel>();
                List<QuizQuestionModel> quizQuestionFullList = QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id);
                if ((quiz.maxNumberQuest == 0) | (quiz.maxNumberQuest >= quizQuestionFullList.Count)) {
                    // 
                    // -- include all questions that have subjects, ordered by subject order
                    foreach (DistanceLearning.Models.QuizSubjectModel subject in quizSubjectList) {
                        foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionFullList) {
                            if ((quizQuestion.subjectID == subject.id))
                                quizQuestionList.Add(quizQuestion);
                        }
                    }
                    // 
                    // -- include all questions with no subjects
                    foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionFullList) {
                        if ((quizQuestion.subjectID == 0))
                            quizQuestionList.Add(quizQuestion);
                    }
                } else {
                    // 
                    // -- include a random set of questions
                    Random random = new Random();
                    if ((quizSubjectList.Count > 0)) {
                        // 
                        // -- subjects included, add maxNumberQuest to each subject
                        foreach (DistanceLearning.Models.QuizSubjectModel subject in quizSubjectList) {
                            // 
                            // -- create list of all questions in the subject
                            List<DistanceLearning.Models.QuizQuestionModel> subjectQuestionList = new List<DistanceLearning.Models.QuizQuestionModel>();
                            foreach (DistanceLearning.Models.QuizQuestionModel question in quizQuestionFullList) {
                                if (question.subjectID == subject.id)
                                    subjectQuestionList.Add(question);
                            }
                            // 
                            // -- add random maxNumberQuest to quizquestionList
                            if ((subjectQuestionList.Count > 0)) {
                                for (int questionPtr = 0; questionPtr <= quiz.maxNumberQuest - 1; questionPtr++) {
                                    if ((subjectQuestionList.Count == 0))
                                        // 
                                        // -- no more questions in this subject
                                        break;
                                    else {

                                        int indexTest = random.Next(0, subjectQuestionList.Count);
                                        DistanceLearning.Models.QuizQuestionModel question = subjectQuestionList[indexTest];
                                        quizQuestionList.Add(question);
                                        subjectQuestionList.Remove(question);
                                    }
                                }
                            }
                        }
                    } else
                        // 
                        // -- no subjects, add maxNumberQuest questions to quiz
                        for (int questionPtr = 0; questionPtr <= quiz.maxNumberQuest - 1; questionPtr++) {
                            int indexTest = random.Next(0, quizQuestionFullList.Count);
                            DistanceLearning.Models.QuizQuestionModel question = quizQuestionFullList[indexTest];
                            quizQuestionList.Add(question);
                            quizQuestionFullList.Remove(question);
                        }
                }
                // 
                // -- first create quiz pages that have subjects
                int pageNumber = 1;
                int detailSortOrder = 0;
                foreach (DistanceLearning.Models.QuizSubjectModel quizSubject in quizSubjectList) {
                    int subjectQuestionCount = 0;
                    foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionList) {
                        if (quizQuestion.subjectID == quizSubject.id) {
                            QuizResponseDetailModel detail = DbBaseModel.addDefault<QuizResponseDetailModel>(cp);
                            detail.questionId = quizQuestion.id;
                            detail.responseId = response.id;
                            detail.pageNumber = pageNumber;
                            detail.sortOrder = quizSubject.name + quizQuestion.sortOrder; // detailSortOrder.ToString.PadLeft(7, "0"c)
                            detail.save(cp);
                            if ((quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage))
                                pageNumber += 1;
                            subjectQuestionCount += 1;
                            detailSortOrder += 1;
                        }
                    }
                    if ((subjectQuestionCount > 0) & (quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneSubjectPerPage))
                        pageNumber += 1;
                }
                // 
                // -- then add pages for questions with no subjects
                foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionList) {
                    bool addDetail = true;
                    if (quizQuestion.subjectID > 0) {
                        bool subjectFound = false;
                        foreach (DistanceLearning.Models.QuizSubjectModel quizSubject in quizSubjectList) {
                            subjectFound = Equals(quizQuestion.subjectID, quizSubject.id);
                            if ((subjectFound))
                                break;
                        }
                        addDetail = !subjectFound;
                    }
                    if (addDetail) {
                        QuizResponseDetailModel detail = DbBaseModel.addDefault<QuizResponseDetailModel>(cp);
                        detail.questionId = quizQuestion.id;
                        detail.responseId = response.id;
                        detail.pageNumber = pageNumber;
                        detail.sortOrder = detailSortOrder.ToString().PadLeft(7, '0');
                        detail.save(cp);
                        if ((quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage))
                            pageNumber += 1;
                        detailSortOrder += 1;
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return response;
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the current quiz response for the current user. if not valid, create a new one
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        public static QuizResponseModel verifyQuizResponse(CPBaseClass cp, QuizModel quiz) {
            //
            // -- get the current user
            var user = DbBaseModel.create<PersonModel>(cp, cp.User.Id);
            if (user == null) { throw new ApplicationException("The current user [" + cp.User.Id + "] is not valid."); }
            return verifyQuizResponse(cp, quiz, user);
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the current quiz response. if not valid, create a new one
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="quizId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static QuizResponseModel verifyQuizResponse(CPBaseClass cp, QuizModel quiz, PersonModel user) {
            //
            // -- once response is created, the response details are expected to be correct
            QuizResponseModel response = QuizResponseModel.createLastForThisUser(cp, quiz.id, user.id);
            if (response == null) {
                return createNewQuizResponse(cp, quiz, user);
            }
            return response;
        }
    }
}

