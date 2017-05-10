using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WebApp_QuartzLab.MyQuartz
{
    public class TestRetryableJob : IJob
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext ctx)
        {
            logger.Info("OnEntry of This is the TestRetryableJab.");
            logger.Info(JsonConvert.SerializeObject(new { ctx.Recovering, ctx.RefireCount, ctx.FireTimeUtc }));

            if(ctx.Recovering)
            {
                logger.Info("被重新執行了。");
            }

            logger.Info("OnLeave of TestRetryableJab.");
        }
    }
}