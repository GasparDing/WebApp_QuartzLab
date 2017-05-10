# WebApp_QuartzLab
請參考：[Quartz.NET 2.5 試用筆記](http://relycoding.blogspot.tw/2017/05/quartznet-25.html)
## 引言
如題，本文章為 Quartz.NET 2.5 試用筆記。
Quartz.NET 來自 java，為一成熟且受迎歡的 open srouce。
排程在工作上常用，解法之前都是寫 windowns service 完全客製化處理。然而近幾年 open source 越來越成熟，功能大致足夠且使用上不難。在時勢與趨勢變遷上，連微軟也向 open source 靠攏的現象，個人認為開始引入成熟的 open source 已是不可擋了。
## 規格要求
這是本人為排程模組訂下的規格。
1. 例行性工作排程；每日固定排程。
2. 可只執行一次。用於因資料狀況而臨時決定執行一次或數次，效果等同背景非同步執行長時間的運算。
3. 可下參數／引數(arguments)。通常臨時執行的job會有引數。
4. 可隱密設定組態。為了安全性不使用可公開編輯的組態檔。
5. 可持續的(persistent)。即執行狀態可存入DB等儲存體。
6. 記Log。
