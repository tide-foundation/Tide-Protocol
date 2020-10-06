az login

$location = Get-Location
$location = $location.Path
$group = "Tide"

$restartApp = {
    param ($number)
    Restart-AzureRmWebApp -ResourceGroupName $group -Name ork-$number
}

For ($i = 0; $i -le 10; $i++) {
    az webapp config appsettings set --resource-group $group --name ork-$i --settings WEBSITE_RUN_FROM_PACKAGE="0"
}

For ($i = 0; $i -le 5; $i++) {
    $pwd = az webapp deployment list-publishing-profiles --name ork-$i --resource-group $group --query '[].userPWD' -o tsv
    $pwd = $pwd[0];
    $project = -join ($location, '\Tide.Ork.csproj');
    $profile = -join ("ork-", $i, " - Web Deploy.pubxml");
    


    dotnet publish $project /p:PublishProfile=$profile /p:Password=$pwd -v q
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Ork" $i "deployed successfully" -ForegroundColor Green
        Start-Job -ArgumentList $i -ScriptBlock $restartApp
    }
    else {
        Write-Host "Ork" $i "failed to deploy" -ForegroundColor Red
    }
}







