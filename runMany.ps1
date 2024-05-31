# Define the API endpoint
$apiUrl = 'https://localhost:55331/api/v1/GateWayControllerV1/proxy'

# Define the headers
$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
$headers.Add("Content-Type", "application/json")
$headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJxWkVsTGJfZEEwMkZ3cU1fNEwtWUxseGNuZWdEdmVMUVZJOGFEZU9NdVRBIn0.eyJleHAiOjE3MTcwNjU2MjgsImlhdCI6MTcxNzA2NTMyOCwianRpIjoiMTgzNzhmMjItMmRmZi00ZjFiLThlMmUtODM4NTU5OTZkMzFiIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy90ZXN0cmVhbG0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiZTZjN2I3MzktNjYyNC00YjI4LWE5ZGEtODMxZjg5MDk3OGJlIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoidGVzdGNsaWVudCIsImFjciI6IjEiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo4MDgwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIiwiZGVmYXVsdC1yb2xlcy10ZXN0cmVhbG0iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6InByb2ZpbGUgZW1haWwiLCJlbWFpbF92ZXJpZmllZCI6ZmFsc2UsImNsaWVudEhvc3QiOiIxNzIuMjYuMC4xIiwicHJlZmVycmVkX3VzZXJuYW1lIjoic2VydmljZS1hY2NvdW50LXRlc3RjbGllbnQiLCJjbGllbnRBZGRyZXNzIjoiMTcyLjI2LjAuMSIsImNsaWVudF9pZCI6InRlc3RjbGllbnQifQ.vm-0MaduS6mSo3oHN1IK17sxWauK4ARNoyOd_YJaKeRW8qoBxjqzWlDPubevgskKIQYeiYhuCQlX_rH2r22nB01YWM58sQ_uMguJi-pqQG-td_Cm0iIh80sDtRWcosFFtdGrBBAQeCJK5lVu63td1sw-ytGsbnlodUcxYzVpHoGITaPCtSsmBBd7_vD3W1oZJa8Nh8nt6Hrb8zQI3E3TMQIOqeIMEB2GPwa8EBvokoCifqlBmnd40SeMxpBMp1YEGbUvszGYOdSk7vnYtaO7NujasuUUPidJcrHq0OZr6gT17d8g9HZBONZ9vRbFydp94o9ow_rS_ValA8JiaEO8Ow")

# Define the body of the request
$body = @"
`"teststring`"
"@

# Start 10 jobs in parallel
$jobs = @()
for ($i = 1; $i -le 10; $i++) {
    $jobs += Start-Job -ScriptBlock {
        param ($Url, $Headers, $Body)
        
        function Send-Request {
            param (
                [string]$Url,
                [hashtable]$Headers,
                [string]$Body
            )
            try {
                $response = Invoke-RestMethod -Uri $Url -Method Post -Headers $Headers -Body $Body
                return $response
            } catch {
                return $_.Exception.Message
            }
        }

        Send-Request -Url $Url -Headers $Headers -Body $Body
    } -ArgumentList $apiUrl, $headers, $body
}

# Wait for all jobs to complete
$jobs | Wait-Job

# Collect and display results
$jobs | ForEach-Object {
    $result = Receive-Job -Job $_
    Write-Host "Response from job $($_.Id): $($result | ConvertTo-Json)"
    Remove-Job -Job $_
}