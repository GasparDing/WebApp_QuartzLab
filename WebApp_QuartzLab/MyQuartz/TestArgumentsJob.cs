using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Newtonsoft.Json;

namespace WebApp_QuartzLab.MyQuartz
{
    public class TestArgumentsJob : IJob
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext ctx)
        {
            logger.Info("OnEntry of TestArgumentsJob.");

            var jobArgs = ctx.JobDetail.JobDataMap;
            var triggerArgs = ctx.Trigger.JobDataMap;
            var mergedArgs = ctx.MergedJobDataMap;

            logger.Info(JsonConvert.SerializeObject(mergedArgs));

            logger.Info("OnLeave of TestArgumentsJob.");
        }
    }
}