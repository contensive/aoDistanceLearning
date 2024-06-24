
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Linq;
using Contensive.Models.Db;
using Contensive.Addons.PortalFramework;

namespace Contensive.Addons.DistanceLearning.Controllers {
    public static class GenericController {
        //
        //====================================================================================================
        /// <summary>
        /// true if argument is numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isNumeric(string value) {
            return value.All(char.IsNumber);
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// buffer an url to include protocol
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string verifyProtocol(string url) {
            // 
            // -- allow empty
            if ((string.IsNullOrWhiteSpace(url)))
                return string.Empty;
            // 
            // -- allow /myPage
            if ((url.Substring(0, 1) == "/"))
                return url;
            // 
            // -- allow if it includes ://
            if ((!url.IndexOf("://").Equals(-1)))
                return url;
            // 
            // -- add http://
            return "http://" + url;
        }
        //
        // create main form tab container
        public static string getTabWrapper(CPBaseClass cp, string innerBody, string activeTabCaption, QuizModel quiz) {
            //PortalFramework.LayoutBuilderSimple formOuter = new PortalFramework.LayoutBuilderSimple();
            //formOuter.isOuterContainer = false;
            // formOuter.title = "Settings";
            //formOuter.title = "this is the title";
            //formOuter.body = innerBody;
            // formOuter.addFormButton("Save", "button");
            // formOuter.addFormButton("Cancel", "button");
            string qs;
            qs = cp.Doc.RefreshQueryString;
            //formOuter.formActionQueryString = qs;
            qs = cp.Utils.ModifyQueryString(qs, "quizId", quiz.id.ToString());
            //
            ContentWithTabsClass tabForm = new ContentWithTabsClass();
            if (!string.IsNullOrEmpty(quiz.name)) {
                tabForm.title = quiz.name;
            } else {
                tabForm.title = "Quiz " + quiz.id;
            }
            tabForm.isOuterContainer = true;
            tabForm.body = innerBody; // formOuter.getHtml(cp);
            //
            tabForm.addTab();
            tabForm.tabCaption = "Details";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails);
            tabForm.tabStyleClass = "";

            //
            tabForm.addTab();
            tabForm.tabCaption = "Settings";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewSetting);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Study";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewStudyPage);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Questions";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionList);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Scoring";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewScoring);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Results";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewResults);
            tabForm.tabStyleClass = "";
            //
            //tabForm.addTab();
            //tabForm.tabCaption = "Results Details";
            //tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResultsDetails);
            //tabForm.tabStyleClass = "";

            tabForm.setActiveTab(activeTabCaption);
            return tabForm.getHtml(cp);
        }
        //
        //=========================================================================
        //  create user error if requestName field is not in doc properties
        //=========================================================================
        //
        public static void checkRequiredFieldText(CPBaseClass cp, string requestName, string fieldCaption) {
            try {
                if (cp.Doc.GetProperty(requestName, "") == "") {
                    cp.UserError.Add("The field " + fieldCaption + " is required.");
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected Error in checkRequiredFieldText");
            }
        }
        //
        //=========================================================================
        //  get the field value, from cs if ok, else from stream
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName, string requestName) {
            string returnValue = "";
            try {
                if (cp.Doc.IsProperty(requestName)) {
                    returnValue = cp.Doc.GetText(requestName, "");
                } else if (cs.OK()) {
                    returnValue = cs.GetText(fieldName);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected Error in getFormField");
            }
            return returnValue;
        }
        //
        //=========================================================================
        //  getFormField, variation
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName) {
            return getFormField(cp, cs, fieldName, fieldName);
        }
        //
        //=========================================================================
        //  if valid date, return the short date, else return blank string 
        //=========================================================================
        //
        public static DateTime encodeMinDate(DateTime src) {
            return src;
        }
        //
        public static string getShortDateString(DateTime srcDate) {
            string returnString = "";
            DateTime workingDate = encodeMinDate(srcDate);
            if (!isDateEmpty(srcDate)) {
                returnString = workingDate.ToShortDateString();
            }
            return returnString;
        }
        //
        public static bool isDateEmpty(DateTime srcDate) {
            return (srcDate < new DateTime(1900, 1, 1));
        }
        public static string getSortOrderFromInteger(int id) {
            return id.ToString().PadLeft(7, '0');
        }
        public static string getDateForHtmlInput(DateTime source) {
            if (isDateEmpty(source)) {
                return "";
            } else {
                return source.Year + "-" + source.Month.ToString().PadLeft(2, '0') + "-" + source.Day.ToString().PadLeft(2, '0');
            }
        }

        // ====================================================================================================
        /// <summary>
        ///         ''' convert string into a style "height: {styleHeight};", if value is numeric it adds "px"
        ///         ''' </summary>
        ///         ''' <param name="styleheight"></param>
        ///         ''' <returns></returns>
        public static string encodeStyleHeight(string styleheight) {
            return string.IsNullOrWhiteSpace(styleheight) ? string.Empty : "overflow:hidden;height:" + styleheight + (Information.IsNumeric(styleheight) ? "px" : string.Empty + ";");
        }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' convert string into a style "background-image: url(backgroundImage)
        ///         ''' </summary>
        ///         ''' <param name="backgroundImage"></param>
        ///         ''' <returns></returns>
        public static string encodeStyleBackgroundImage(CPBaseClass cp, string backgroundImage) {
            return string.IsNullOrWhiteSpace(backgroundImage) ? string.Empty : "background-image: url(" + cp.Site.FilePath + backgroundImage + ");";
        }

        // 
        //        internal static DateTime encodeMinDate(DateTime sourceDate) {
        //            DateTime returnValue = sourceDate;
        //            ;/* Cannot convert MultiLineIfBlockSyntax, System.ArgumentOutOfRangeException: Exception of type 'System.ArgumentOutOfRangeException' was thrown.
        //Parameter name: value
        //Actual value was 1/1/1900 12:00:00 AM.
        //   at ICSharpCode.CodeConverter.CSharp.CommonConversions.GetLiteralExpression(Object value, String valueText)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

        //Input: 
        //            If returnValue < #1/1/1900# Then
        //                returnValue = Date.MinValue
        //            End If

        // */
        //            return returnValue;
        //        }
        // 
        // 
        // 
        //        internal static string encodeShortDateString(DateTime sourceDate) {
        //            string returnValue;
        //            ;/* Cannot convert MultiLineIfBlockSyntax, System.ArgumentOutOfRangeException: Exception of type 'System.ArgumentOutOfRangeException' was thrown.
        //Parameter name: value
        //Actual value was 1/1/1900 12:00:00 AM.
        //   at ICSharpCode.CodeConverter.CSharp.CommonConversions.GetLiteralExpression(Object value, String valueText)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

        //Input: 
        //            '
        //            If sourceDate < #1/1/1900# Then
        //                returnValue = ""
        //            Else
        //                returnValue = sourceDate.ToShortDateString
        //            End If

        // */
        //            return returnValue;
        //        }
        // 
        // 
        // 
        internal static string encodeBlankCurrency(double source) {
            string returnValue = "";
            if (source != 0)
                returnValue = source.ToString("C", CultureInfo.CurrentCulture);
            return returnValue;
        }

        public static string addEditWrapper(CPBaseClass cp, string innerHtml, int recordId, string recordName, string contentName) {
            if (!cp.User.IsEditingAnything) {
                return innerHtml;
            }
            string header = cp.Content.GetEditLink(contentName, recordId.ToString(), false, recordName, true);
            string content = cp.Html.div(innerHtml, "", "");
            string miniWrapperCSS = "style=\"white-space: nowrap; text-align: left; box-sizing: border-box; color: white !important; z-index: 1; text-decoration: none !important; display: inline-block !important; list-style-type: none !important; margin: 0 auto auto 0 !important; padding: 2px 10px !important; font-size: 1rem !important; font-weight: 400 !important; line-height: 1.5 !important;\"";
            return cp.Html.div(header + content, "", "ccEditWrapper");
        }
    }
}

