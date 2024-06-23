using System;
using System.ComponentModel;

namespace Droplet.Data.Enum
{
    public enum BloodTypeEnum
    {
        [Description("0Rh+")]
        O_Positive,
        [Description("0Rh-")]
        O_Negative,
        [Description("ARh+")]
        A_Positive,
        [Description("ARh-")]
        A_Negative,
        [Description("BRh+")]
        B_Positive,
        [Description("BRh-")]
        B_Negative,
        [Description("ABRh+")]
        AB_Positive,
        [Description("ABRh-")]
        AB_Negative
    }


}
