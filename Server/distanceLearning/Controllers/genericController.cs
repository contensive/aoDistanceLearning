
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning.Controllers
{
    static class genericController
    {
        //
        //=========================================================================
        //  create user error if requestName field is not in doc properties
        //=========================================================================
        //
        public static void checkRequiredFieldText(CPBaseClass cp, string requestName, string fieldCaption)
        {
            try
            {
                if (cp.Doc.GetProperty(requestName, "") == "")
                {
                    cp.UserError.Add("The field " + fieldCaption + " is required.");
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpected Error in checkRequiredFieldText");
            }
        }
        //
        //=========================================================================
        //  get the field value, from cs if ok, else from stream
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName, string requestName)
        {
            string returnValue = "";
            try
            {
                if (cp.Doc.IsProperty(requestName))
                {
                    returnValue = cp.Doc.GetText(requestName, "");
                }
                else if (cs.OK())
                {
                    returnValue = cs.GetText(fieldName);
                }
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport(ex, "Unexpected Error in getFormField");
            }
            return returnValue;
        }
        //
        //=========================================================================
        //  getFormField, variation
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName)
        {
            return getFormField(cp, cs, fieldName, fieldName);
        }
        //
        //=========================================================================
        //  if date is invalid, set to minValue
        //=========================================================================
        //
        public static DateTime encodeMinDate(DateTime srcDate)
        {
            DateTime returnDate = srcDate;
            if (srcDate < new DateTime(1900, 1, 1))
            {
                returnDate = DateTime.MinValue;
            }
            return returnDate;
        }
        //
        //=========================================================================
        //  if valid date, return the short date, else return blank string 
        //=========================================================================
        //
        public static string getShortDateString(DateTime srcDate)
        {
            string returnString = "";
            DateTime workingDate = encodeMinDate( srcDate );
            if (srcDate > new DateTime(1900, 1, 1))
            {
                returnString = workingDate.ToShortDateString();
            }
            return returnString;
        }
    }
}
