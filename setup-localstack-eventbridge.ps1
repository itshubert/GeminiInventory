#!/usr/bin/env pwsh
# LocalStack EventBridge Setup Script for GeminiInventory

$ErrorActionPreference = "Stop"
$endpoint = "http://localhost:4566"
$region = "us-east-1"

Write-Host "🚀 Setting up LocalStack EventBridge for GeminiInventory..." -ForegroundColor Cyan

# Step 1: Create SQS FIFO Queue
Write-Host "`n📦 Creating SQS FIFO Queue..." -ForegroundColor Yellow
try {
    $queueUrl = aws --endpoint-url=$endpoint --region $region sqs create-queue `
        --queue-name inventory-level-changed.fifo `
        --attributes FifoQueue=true,ContentBasedDeduplication=true `
        --output text --query 'QueueUrl' 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ SQS Queue created: $queueUrl" -ForegroundColor Green
    } else {
        Write-Host "   ⚠ Queue might already exist, continuing..." -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠ Error creating queue (might already exist): $_" -ForegroundColor Yellow
}

# Step 2: Create EventBridge Event Bus
Write-Host "`n🎯 Creating EventBridge Event Bus 'gemini'..." -ForegroundColor Yellow
try {
    $eventBus = aws --endpoint-url=$endpoint --region $region events create-event-bus `
        --name gemini `
        --output text 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Event Bus created successfully" -ForegroundColor Green
    } else {
        Write-Host "   ⚠ Event Bus might already exist, continuing..." -ForegroundColor Yellow
    }
} catch {
    Write-Host "   ⚠ Error creating event bus (might already exist): $_" -ForegroundColor Yellow
}

# Step 3: Create EventBridge Rule
Write-Host "`n📋 Creating EventBridge Rule 'InventoryLevelChangedRule'..." -ForegroundColor Yellow
try {
    $rule = aws --endpoint-url=$endpoint --region $region events put-rule `
        --name InventoryLevelChangedRule `
        --event-bus-name gemini `
        --event-pattern '{\"source\":[\"gemini\"],\"detail-type\":[\"InventoryLevelChanged\"]}' `
        --output text 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Rule created successfully" -ForegroundColor Green
    } else {
        throw "Failed to create rule: $rule"
    }
} catch {
    Write-Host "   ✗ Error creating rule: $_" -ForegroundColor Red
    exit 1
}

# Step 4: Add SQS Target to Rule
Write-Host "`n🎯 Adding SQS target to rule..." -ForegroundColor Yellow
try {
    $target = aws --endpoint-url=$endpoint --region $region events put-targets `
        --rule InventoryLevelChangedRule `
        --event-bus-name gemini `
        --targets '[{\"Id\":\"sqs-inventory-level-changed\",\"Arn\":\"arn:aws:sqs:us-east-1:000000000000:inventory-level-changed.fifo\",\"SqsParameters\":{\"MessageGroupId\":\"inventory-level-changes\"}}]' `
        --output text 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ✓ Target added successfully" -ForegroundColor Green
    } else {
        throw "Failed to add target: $target"
    }
} catch {
    Write-Host "   ✗ Error adding target: $_" -ForegroundColor Red
    exit 1
}

# Step 5: Verify Setup
Write-Host "`n🔍 Verifying setup..." -ForegroundColor Yellow

Write-Host "`n   Event Buses:" -ForegroundColor Cyan
aws --endpoint-url=$endpoint --region $region events list-event-buses --output table

Write-Host "`n   Rules on 'gemini' event bus:" -ForegroundColor Cyan
aws --endpoint-url=$endpoint --region $region events list-rules --event-bus-name gemini --output table

Write-Host "`n   Targets for 'InventoryLevelChangedRule':" -ForegroundColor Cyan
aws --endpoint-url=$endpoint --region $region events list-targets-by-rule --rule InventoryLevelChangedRule --event-bus-name gemini --output table

Write-Host "`n✅ LocalStack EventBridge setup complete!" -ForegroundColor Green
Write-Host "`n📝 You can now test by publishing events to the 'gemini' event bus" -ForegroundColor Cyan
