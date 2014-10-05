Add-Type -Path "C:\Workspaces\GitHub\TestR\TestR.Data\bin\Debug\TestR.Data.dll"
$connectionString = "Server=localhost;Database=TestR;User Id=TestR;Password=APOXIhb5MRUi"
$dataContext = New-Object -TypeName "TestR.Data.DataContext" -ArgumentList $connectionString
$logger = New-Object -TypeName "TestR.Logging.DataContextLogger" -ArgumentList $dataContext