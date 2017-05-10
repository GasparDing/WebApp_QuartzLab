using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;

namespace WebApp_QuartzLab.MyQuartz
{
    public class SimpleJob : IJob
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext ctx)
        {
            logger.Info("Hello from SimpleJob");
        }
    }
}