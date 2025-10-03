#!/usr/bin/env pwsh
# Test EventBridge to SQS flow

$endpoint = "http://localhost:4566"
$region = "us-east-1"

Write-Host "🧪 Testing EventBridge to SQS flow..." -ForegroundColor Cyan

# Send test event to EventBridge
Write-Host "`n📤 Sending test event to EventBridge..." -ForegroundColor Yellow

# Create the event payload
$eventDetail = @{
    inventoryId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    productId = "550e8400-e29b-41d4-a716-446655440000"
    quantityAvailable = 100
    quantityReserved = 10
} | ConvertTo-Json -Compress

$eventEntry = @{
    Source = "gemini"
    DetailType = "InventoryLevelChanged"
    Detail = $eventDetail
    EventBusName = "gemini"
} | ConvertTo-Json -Compress

$entries = "[$eventEntry]"

# Write to temp file to avoid escaping issues
$tempFile = [System.IO.Path]::GetTempFileName()
$entries | Out-File -FilePath $tempFile -Encoding utf8 -NoNewline

$result = aws --endpoint-url=$endpoint --region $region events put-events --entries file://$tempFile 2>&1

Remove-Item $tempFile -Force

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Event sent successfully" -ForegroundColor Green
    $result | ConvertFrom-Json | ConvertTo-Json -Depth 5
} else {
    Write-Host "   ✗ Failed to send event" -ForegroundColor Red
    Write-Host $result
    exit 1
}

# Wait a moment for EventBridge to process
Write-Host "`n⏳ Waiting 2 seconds for EventBridge to route the event..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Check SQS queue for messages
Write-Host "`n📬 Checking SQS queue for messages..." -ForegroundColor Yellow
$queueUrl = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/inventory-level-changed.fifo"

$messages = aws --endpoint-url=$endpoint --region $region sqs receive-message --queue-url $queueUrl --max-number-of-messages 10 2>&1

if ($LASTEXITCODE -eq 0) {
    $messageJson = $messages | ConvertFrom-Json
    if ($messageJson.Messages) {
        Write-Host "   ✓ Found $($messageJson.Messages.Count) message(s) in queue!" -ForegroundColor Green
        Write-Host "`n📨 Message details:" -ForegroundColor Cyan
        $messages | ConvertTo-Json -Depth 10
    } else {
        Write-Host "   ⚠ No messages found in queue" -ForegroundColor Yellow
        Write-Host "   This could mean:" -ForegroundColor Gray
        Write-Host "     1. EventBridge rule didn't match the event" -ForegroundColor Gray
        Write-Host "     2. LocalStack FIFO queue issue (check docker logs)" -ForegroundColor Gray
        Write-Host "     3. Event was sent but not routed to SQS" -ForegroundColor Gray
    }
} else {
    Write-Host "   ✗ Error checking queue" -ForegroundColor Red
    Write-Host $messages
}

Write-Host "`n💡 Check LocalStack logs with: docker logs localstack --tail 50" -ForegroundColor Cyan
