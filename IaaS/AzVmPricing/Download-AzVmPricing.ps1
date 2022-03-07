$region = "northeurope"
$Path= "C:\temp\AzComputeResourceSku.csv"
(Get-AzComputeResourceSku -Location "northeurope"   ) | where {$_.ResourceType -eq "virtualMachines" } | Select-Object @{Name='vmName';   Expression={ $_.Name } ; } -ExpandProperty Capabilities | Export-Csv -Path $Path



$Path= "C:\temp\AzCost.csv"
if (Test-Path $Path) {
  Remove-Item $Path
}
$Uri='https://prices.azure.com/api/retail/prices?currencyCode=''EUR''&$filter=serviceName eq ''Virtual Machines'' and priceType eq ''Consumption'' and armRegionName eq ''northeurope'''
Do {
    $Uri
    $page=Invoke-RestMethod -Uri $Uri 
    $Count=$page.Count
    $Uri=$page.NextPageLink
    $page.Items |  Select-Object CurrencyCode, armSkuName , productId, skuId, location, retailPrice, type ,  unitOfMeasure   | Export-Csv -Path $Path  -Append

} while ($Uri)