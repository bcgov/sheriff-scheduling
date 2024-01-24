Add-Migration Migration
Update-Database
Remove-Migration

When running in Package Manager Console inside of Visual Studio:
Make sure that the api project is set as the startup project, with the default project set to db.

====================================================================================================
EF Core:

This was also another key piece of documentation I found along the way:

https://docs.microsoft.com/en-us/ef/core/querying/related-data/eager

In case of tracking queries, results of Filtered Include may be unexpected due to navigation fixup. 
All relevant entities that have been queried for previously and have been stored in the Change Tracker will be present in the results of 
Filtered Include query, even if they don't meet the requirements of the filter. Consider using NoTracking queries or re-create the DbContext 
when using Filtered Include in those situations.

=====================================================================================================
Timezones:

https://www.npgsql.org/doc/types/nodatime.html

