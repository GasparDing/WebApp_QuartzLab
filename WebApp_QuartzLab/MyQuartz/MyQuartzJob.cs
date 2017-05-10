using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;

namespace WebApp_QuartzLab.MyQuartz
{
    public class MyQuartzJob : Quartz.IJob
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        void IJob.Execute(IJobExecutionContext ctx)
        {
            logger.Trace("ON : MyJob.Execute...");
            logger.Info("現在時間是：" + DateTime.Now);
        }
    }
}