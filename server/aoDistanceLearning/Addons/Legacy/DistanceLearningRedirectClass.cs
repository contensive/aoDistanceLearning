
using System;
using Contensive.BaseClasses;

namespace Contensive.Addons.DistanceLearning.Addons.Legacy {
    public class DistanceLearningRedirectClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                cp.Response.Redirect("?addonGuid={A1BCA00C-2657-42A1-8F98-BED1E5A42025}&setPortalGuid={771bfee5-1fa7-4edf-8b6c-1406ab7bacf9}");
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "DistanceLearningRedirectClass.Execute");
            }
            return result;
        }
    }
}
