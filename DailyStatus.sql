--select * from DailyStatus order by DateOfInsert Desc
--update DailyStatus set dateofinsert = '2019-05-07' where dateofinsert = '2019-05-08' and Name = 'Prakash'
--Delete from DailyStatus where name = 'test' and DateOfInsert = '2019-05-01'
--Drop table DailyStatus
--Drop procedure InsertData

CREATE DATABASE WorkStatus;

create table DailyStatus(
	Name varchar(20) NOT NULL,
	SoFar varchar(MAX) NOT NULL,
	NextDay varchar(MAX) NOT NULL,
	Impediments varchar(MAX) NOT NULL,
	DateOfInsert date NOT NULL,
	primary key (Name, DateOfInsert),
	DateOfUpdate datetime,
	UpdatedBy varchar(120)
)

ALTER TABLE DailyStatus
   ADD ID INT IDENTITY

--drop proc InsertData
Create Procedure InsertData  
(  
@Name varchar(20),  
@SoFar varchar(MAX),  
@NextDay varchar(MAX),  
@Impediments  varchar(MAX) ,
@Overwrite int
)  
as begin   
	if(@Overwrite = 0)
		Insert into DailyStatus(Name, SoFar, NextDay, Impediments, DateOfInsert, DateOfUpdate) values(@Name,@SoFar,@NextDay,@Impediments,GETDATE(), GETDATE())  
	else if(@Overwrite = -1)
		Update DailyStatus set SoFar = @SoFar, NextDay = @NextDay, Impediments = @Impediments, DateOfUpdate = GETDATE() where Name = @Name and DateOfInsert = convert(varchar, getdate(), 23)
	else 
		Update DailyStatus set SoFar = @SoFar, NextDay = @NextDay, Impediments = @Impediments, DateOfUpdate = GETDATE() where ID = @Overwrite
End  

Create Procedure GetStatusForSelectedDate 
(  
	@DateOfStatus  date
)  
as begin   
	select ID, Name, SoFar, NextDay, Impediments 
	from DailyStatus where DateOfInsert = CONVERT(date, @DateOfStatus)
End  

Create Procedure GetAllDates 
as begin   
select distinct(DateOfInsert) from DailyStatus order by DateOfInsert desc
End 

Create Procedure SearchString  
(  
	@FilterString varchar(MAX),
	@NameFilter BIT,
	@SoFarFilter BIT,
	@NextDayFilter BIT,
	@ImpedimentsFilter BIT
)  
as begin  
	DECLARE @SQLString nvarchar(MAX);
	SET @SQLString = N'select * from DailyStatus where 1 = 0';
	if(@NameFilter = 1)
		SET @SQLString = @SQLString + N' OR Name like ''%' + @FilterString + '%''';
	if(@SoFarFilter = 1)
		SET @SQLString = @SQLString + N' OR SoFar like ''%' + @FilterString + '%''';
	if(@NextDayFilter = 1)
		SET @SQLString = @SQLString + N' OR NextDay like ''%' + @FilterString + '%''';
	if(@ImpedimentsFilter = 1)
		SET @SQLString = @SQLString + N' OR Impediments like ''%' + @FilterString + '%''';
	SET @SQLString = @SQLString + N' ORDER BY DateOfInsert DESC'
	--print @SQLString
	EXEC(@SQLString)	
End 


-----------WEEKLY-------------------------
--select top 100 * from weeklystatus order by DateOfInsert Desc
--update weeklystatus set dateofinsert = '2019-05-13' where dateofinsert = '2019-06-03' and AssignTo = 'Aravinda'
--update WeeklyStatus set taskstatus='In Progress' where taskstatus = 'InProgress'
--delete from WeeklyStatus where AssignTo = 'Rituraj' and DateOfInsert = '2019-06-10'
--select * from WeeklyStatus order by DateOfUpdate desc

--drop table WeeklyStatus
create table WeeklyStatus(
	Task varchar(120) NOT NULL,
	AssignTo varchar(20) NOT NULL,
	TaskPriority int NOT NULL,
	Complexity varchar(30) NOT NULL,
	TaskStatus varchar(30) NOT NULL,
	Comments varchar(MAX),
	DateOfInsert date NOT NULL,
	primary key (Task, AssignTo, DateOfInsert),
	DateOfUpdate datetime,
	UpdatedBy varchar(120)
)

ALTER TABLE WeeklyStatus
   ADD ID INT IDENTITY

--drop proc InsertWeeklyData
create Procedure InsertWeeklyData  
(  
@Task varchar(120),
@AssignTo varchar(20),
@TaskPriority int,
@Complexity varchar(30),
@TaskStatus varchar(30),
@Comments varchar(MAX),
@Overwrite int
)  
as begin   
	if(@Overwrite = 0)
		Insert into WeeklyStatus(Task, AssignTo, TaskPriority, Complexity, TaskStatus, Comments, DateOfInsert, DateOfUpdate) 
		values(@Task,@AssignTo,@TaskPriority,@Complexity,@TaskStatus,@Comments, DATEADD(wk, DATEDIFF(wk,0,GETDATE()), 0), GETDATE())  
	else if(@Overwrite = -1)
		Update WeeklyStatus set TaskPriority = @TaskPriority, Complexity = @Complexity, 
		TaskStatus = @TaskStatus, Comments = @Comments, DateOfUpdate = GETDATE()
		where Task = @Task and AssignTo = @AssignTo and DateOfInsert = convert(varchar, DATEADD(wk, DATEDIFF(wk,0,GETDATE()), 0), 23)
	else
		Update WeeklyStatus set TaskPriority = @TaskPriority, Complexity = @Complexity, 
		TaskStatus = @TaskStatus, Comments = @Comments, Task = @Task, DateOfUpdate = GETDATE() where ID =@Overwrite
End  

create Procedure InsertWeeklyDataWithDate 
(  
@Task varchar(120),
@AssignTo varchar(20),
@TaskPriority int,
@Complexity varchar(30),
@TaskStatus varchar(30),
@Comments varchar(MAX),
@Overwrite int,
@WeekDate datetime
)  
as begin   
	if(@Overwrite = 0)
		Insert into WeeklyStatus(Task, AssignTo, TaskPriority, Complexity, TaskStatus, Comments, DateOfInsert, DateOfUpdate) 
		values(@Task,@AssignTo,@TaskPriority,@Complexity,@TaskStatus,@Comments, @WeekDate, GETDATE())  
	else if(@Overwrite = -1)
		Update WeeklyStatus set TaskPriority = @TaskPriority, Complexity = @Complexity, 
		TaskStatus = @TaskStatus, Comments = @Comments, DateOfUpdate = GETDATE()
		where Task = @Task and AssignTo = @AssignTo and DateOfInsert = convert(varchar, DATEADD(wk, DATEDIFF(wk,0,GETDATE()), 0), 23)
	else
		Update WeeklyStatus set TaskPriority = @TaskPriority, Complexity = @Complexity, 
		TaskStatus = @TaskStatus, Comments = @Comments, Task = @Task, DateOfUpdate = GETDATE() where ID =@Overwrite
End  

create Procedure GetStatusForSelectedWeek 
(  
	@DateOfStatus  date
)  
as begin   
	select ROW_NUMBER() OVER(ORDER BY AssignTo asc,Task ASC) AS RNo, ID, Task, AssignTo, TaskPriority, Complexity, TaskStatus, Comments 
	from WeeklyStatus where DateOfInsert = CONVERT(date, DATEADD(wk, DATEDIFF(wk,0,@DateOfStatus), 0))
End  

Create Procedure GetAllWeeklyDates 
as begin   
select distinct(DateOfInsert) from WeeklyStatus order by DateOfInsert desc
End 

--SELECT DATEADD(wk, DATEDIFF(wk,0,GETDATE()), 0) MondayOfCurrentWeek


-----------WEEKLY  BRIEF-------------------------


--select * from WeeklyBrief
--update WeeklyBrief set DateOfInsert = '2019-01-21' where DateOfInsert = '2019-01-28'
--delete from WeeklyBrief where DateOfInsert = '2019-01-28'
create table WeeklyBrief(
	Brief varchar(MAX),
	DateOfInsert date primary key,
	DateOfUpdate datetime,
	UpdatedBy varchar(120)
)

create Procedure InsertWeeklyBrief
(  
	@Brief varchar(MAX),
	@BriefDate date
)  
as begin   
	if exists(select 1 from WeeklyBrief
              where DateOfInsert = CONVERT(date, DATEADD(wk, DATEDIFF(wk,0,@BriefDate), 0))) 
	begin
		update WeeklyBrief set Brief = @Brief, DateOfUpdate = GETDATE()
		where DateOfInsert = CONVERT(date, DATEADD(wk, DATEDIFF(wk,0,@BriefDate), 0)) 
	end
	else
	begin
		insert into WeeklyBrief(Brief, DateOfInsert,DateOfUpdate) values(@Brief, DATEADD(wk, DATEDIFF(wk,0,@BriefDate), 0),GETDATE())
	end
End  

Create Procedure GetBriefForSelectedWeek 
(  
	@DateOfStatus  date
)  
as begin   
	select Brief from WeeklyBrief 
	where DateOfInsert = CONVERT(date, DATEADD(wk, DATEDIFF(wk,0,@DateOfStatus), 0))
End