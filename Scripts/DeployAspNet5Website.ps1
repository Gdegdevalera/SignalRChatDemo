<#
    .SYNOPSIS 
    Publishes an ASP.NET MVC 6 Windows Azure website project
            
    .DESCRIPTION
    The Publish-AspNet5Website.ps1 script publishes
    the project that is associated with a Windows Azure
    website. 

    To run this script, you must have a Windows Azure 
    website associated with your Windows Azure account.
    To verify, run the Get-AzureWebsite cmdlet.

	This script assumes that dependent script PublishAspNet5Website.ps1 is located in the same folder.
           
    .PARAMETER  ProjectFilePath
    Specifies the project.json file of the project that you
    want to deploy. This parameter is required.

	.PARAMETER  PublishOutputPath
    Specifies the publish output path

	.PARAMETER  WebsiteName
    Specifies the web site name in Windows Azure in which to publish to

    .PARAMETER  Launch
    Starts a browser that displays the website. This
    switch parameter is optional.

    .INPUTS
    System.String

    .OUTPUTS
    None. This script does not return any objects.

	.EXAMPLE
	DeployAspNet5Website.ps1 "C:\Users\<userName>\Documents\Visual Studio 2015\Projects\<SolutionName>\src\<ProjectName>" -PublishOutputPath "C:\temp\output\" -WebsiteName webSiteName

    .LINK
    Show-AzureWebsite
#> 
Param(
    [Parameter(Mandatory = $true)]
    [String]$ProjectFilePath,
	[String]$PublishOutputPath,
	[String]$WebsiteName
    #[Switch]$Launch
)

# Get the current script path, we will return to this.
$scriptPath = (Get-Item -Path ".\" -Verbose).FullName

Write-Host "Current path is"$scriptPath -foregroundcolor "White"

Write-Host "Changing directory to" $ProjectFilePath  -foregroundcolor "White"

cd $ProjectFilePath

#Is building even requred?
Write-Host "Building solution" -foregroundcolor "Yellow"
#dnu build

Write-Host "Publishing to" $PublishOutputPath -foregroundcolor "Yellow"
dnu publish --out "$PublishOutputPath" --runtime active

Write-Host "Returning to script path" -foregroundcolor "White"
cd $scriptPath

Write-Host "Deploying to Azure" -foregroundcolor "Yellow"
.\PublishAspNet5Website.ps1 $WebsiteName -packOutput $PublishOutputPath

