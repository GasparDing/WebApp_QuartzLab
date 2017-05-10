using Quartz;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApp_QuartzLab
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IScheduler _schedular = null; // Quartz排程器

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //## for Quarzt.NET
            // 建立排程器
            ISchedulerFactory sf = new Quartz.Impl.StdSchedulerFactory(QuartzConfig());
            _schedular = sf.GetScheduler();

            _schedular.Start(); // 啟動Quartz排程器

            #region Quarzt-建立工作與排程

            //# 建立固定排程工作１,原則上一個Job呼應一個Trigger。
            IJobDetail job1 = JobBuilder.Create<WebApp_QuartzLab.MyQuartz.MyQuartzJob>()
                                       .WithIdentity("job1")
                                       .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                                    .WithIdentity("trigger1")
                                    .WithCronSchedule("0/5 * * * * ?")  // 每5秒觸發一次。
                                    .Build();

            _schedular.ScheduleJob(job1, trigger1);

            //# 建立固定排程工作２,原則上一個Job呼應一個Trigger。
            IJobDetail job2 = JobBuilder.Create<WebApp_QuartzLab.MyQuartz.SimpleJob>()
                                        .WithIdentity("job2", "MyGroup")
                                        .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                                    .WithIdentity("trigger2", "MyGroup")
                                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()) // 每間隔10秒執行一次，不斷重複。
                                    .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Second)) // 5秒後開始執行
                                    .EndAt(DateBuilder.FutureDate(2, IntervalUnit.Minute)) // 2分後停止執行
                                    .Build();

            _schedular.ScheduleJob(job2, trigger2);

            //# 先只建立Job
            IJobDetail job3 = JobBuilder.Create<WebApp_QuartzLab.MyQuartz.TestRetryableJob>()
                                        .WithIdentity("Job3")
                                        .WithDescription("Hi. This is the Job3.")
                                        .Build();

            _schedular.AddJob(job3, true, true);

            // 再建立trigger 與已存在的Job掛勾。多個 trigger 可與同一個 job 掛勾
            ITrigger trigger3 = TriggerBuilder.Create()
                                    .ForJob("Job3") // 
                                    .WithIdentity("trigger3")
                                    .WithSimpleSchedule(s => s.WithIntervalInSeconds(4).WithRepeatCount(2)) // 每隔４秒重複２次，也就是總共跑３次
                                    .StartAt(DateBuilder.FutureDate(7, IntervalUnit.Second)) // 7秒後開始執行
                                    .Build();

            ITrigger trigger4 = TriggerBuilder.Create()
                                    .ForJob("Job3")
                                    .WithIdentity("trigger4")
                                    .StartAt(DateBuilder.DateOf(14, 6, 0)) // 於指定時間 14:06:00 執行，若時間已過將會立刻執行。將只會執行一次。
                                    .Build();

            _schedular.ScheduleJob(trigger3);
            _schedular.ScheduleJob(trigger4);


            // 傳入參數給排程，Job 與 trigger 都給參數
            IJobDetail job5 = JobBuilder.Create<WebApp_QuartzLab.MyQuartz.TestArgumentsJob>()
                                        .WithIdentity("job5", "MyGroup")
                                        .UsingJobData("JobArg1", "aaaaa") // 參數
                                        .UsingJobData("JobArg2", "bbbbb") // 同名參數-預設效果
                                        .Build();

            ITrigger trigger5 = TriggerBuilder.Create()
                                    .WithIdentity("trigger5", "MyGroup")
                                    .StartAt(DateBuilder.FutureDate(6, IntervalUnit.Second)) // 5秒後開始執行
                                    .UsingJobData("JobArg2", "xxxxx")　// 同名參數-後來為主
                                    .UsingJobData("JobArg3", "ccccc")
                                    .Build();

            _schedular.ScheduleJob(job5, trigger5);

            // 傳入參數給排程 - 化簡程式碼
            _schedular.ScheduleJob(JobBuilder.Create<WebApp_QuartzLab.MyQuartz.TestArgumentsJob>()
                                    .Build()
                                  ,TriggerBuilder.Create()
                                    .StartAt(DateBuilder.FutureDate(12, IntervalUnit.Second)) // 5秒後開始執行
                                    .UsingJobData("JobArg1", "AAA") // 參數
                                    .UsingJobData("JobArg2", "XXX")
                                    .UsingJobData("JobArg3", "CCC")
                                    .Build());

            #endregion
            
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _schedular.Shutdown(false); // 停止Quartz排程器
        }

        protected NameValueCollection QuartzConfig()
        {
            NameValueCollection quartzProps = new NameValueCollection();

            quartzProps.Add("quartz.scheduler.instanceName","MyScheduler");
            quartzProps.Add("quartz.scheduler.instanceId","AUTO");
            quartzProps.Add("quartz.threadPool.type","Quartz.Simpl.SimpleThreadPool, Quartz");
            quartzProps.Add("quartz.threadPool.threadCount", "10");
            quartzProps.Add("quartz.threadPool.threadPriority","Normal");

            quartzProps.Add("quartz.scheduler.idleWaitTime","5000");

            quartzProps.Add("quartz.jobStore.misfireThreshold","60000");
            quartzProps.Add("quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX, Quartz");
            quartzProps.Add("quartz.jobStore.tablePrefix","QRTZ_");
            quartzProps.Add("quartz.jobStore.clustered","false");
            quartzProps.Add("quartz.jobStore.driverDelegateType","Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz");
            quartzProps.Add("quartz.jobStore.lockHandler.type","Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz");
            quartzProps.Add("quartz.jobStore.useProperties","true");

            quartzProps.Add("quartz.jobStore.dataSource","default");
            quartzProps.Add("quartz.dataSource.default.connectionString",@"Data Source=relynb\sqlexpress;Initial Catalog=Quartz;Integrated Security=True");
            quartzProps.Add("quartz.dataSource.default.provider","SqlServer-20");

            return quartzProps;
        }
    }
    
}
