#!/usr/bin/env pwsh
# LocalStack EventBridge Cleanup Script for GeminiInventory

$ErrorActionPreference = "Continue"
$endpoint = "http://localhost:4566"
$region = "us-east-1"

Write-Host "🧹 Cleaning up LocalStack EventBridge resources..." -ForegroundColor Cyan

# Step 1: Remove targets from rule
Write-Host "`n🎯 Removing targets from 'InventoryLevelChangedRule'..." -ForegroundColor Yellow
try {
    # First, list targets to see what exists
    $targets = aws --endpoint-url=$endpoint --region $region events list-targets-by-rule `
        --rule InventoryLevelChangedRule `
        --event-bus-name gemini `
        --query 'Targets[].Id' `
        --output text 2>&1
    
    if ($LASTEXITCODE -eq 0 -and $targets) {
        Write-Host "   Found targets: $targets" -ForegroundColor Gray
        
        # Try to remove targets (this is the problematic command)
        $result = aws --endpoint-url=$endpoint --region $region events remove-targets `
            --rule InventoryLevelChangedRule `
            --event-bus-name gemini `
            --ids $targets.Split() 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "   ✓ Targets removed successfully" -ForegroundColor Green
        } else {
            Write-Host "   ⚠ Failed to remove targets (will delete rule anyway): $result" -ForegroundColor Yellow
        }
    } else {
        Write-Host "   ℹ No targets found or rule doesn't exist" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ⚠ Error: $_" -ForegroundColor Yellow
}

# Step 2: Delete the rule
Write-Host "`n📋 Deleting rule 'InventoryLevelChangedRule'..." -ForegroundColor Yellow
try {
    $result = aws --endpoint-url=$endpoint --region $region events delete-rule `
        --name InventoryLevelChangedRule `
        --event-bus-name gemini 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Rule deleted successfully" -ForegroundColor Green
    } else {
        Write-Host "   ⚠ Rule might not exist: $result" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠ Error: $_" -ForegroundColor Yellow
}

# Step 3: Delete the event bus
Write-Host "`n🗑️  Deleting event bus 'gemini'..." -ForegroundColor Yellow
try {
    $result = aws --endpoint-url=$endpoint --region $region events delete-event-bus `
        --name gemini 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Event bus deleted successfully" -ForegroundColor Green
    } else {
        Write-Host "   ⚠ Event bus might not exist: $result" -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠ Error: $_" -ForegroundColor Yellow
}

# Step 4: Delete the SQS queue
Write-Host "`n📦 Deleting SQS queue 'inventory-level-changed.fifo'..." -ForegroundColor Yellow
try {
    # Get queue URL first
    $queueUrl = aws --endpoint-url=$endpoint --region $region sqs get-queue-url `
        --queue-name inventory-level-changed.fifo `
        --output text --query 'QueueUrl' 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        $result = aws --endpoint-url=$endpoint --region $region sqs delete-queue `
            --queue-url $queueUrl 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "   ✓ SQS queue deleted successfully" -ForegroundColor Green
        } else {
            Write-Host "   ⚠ Failed to delete queue: $result" -ForegroundColor Yellow
        }
    } else {
        Write-Host "   ℹ Queue doesn't exist" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ⚠ Error: $_" -ForegroundColor Yellow
}

# Step 5: Verify cleanup
Write-Host "`n🔍 Verifying cleanup..." -ForegroundColor Yellow

Write-Host "`n   Remaining Event Buses:" -ForegroundColor Cyan
aws --endpoint-url=$endpoint --region $region events list-event-buses --output table

Write-Host "`n✅ Cleanup complete!" -ForegroundColor Green
Write-Host "`n💡 Tip: If you see persisted state errors, restart LocalStack with:" -ForegroundColor Cyan
Write-Host "   docker restart localstack" -ForegroundColor White
