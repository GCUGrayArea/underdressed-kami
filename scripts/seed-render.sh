#!/bin/bash
# Seed demo data to Render deployment
# Run this with: bash scripts/seed-render.sh

API_URL="https://smartscheduler-backend-7mth.onrender.com/api"

# Job Type IDs (from migration seed)
TILE_INSTALLER="10000000-0000-0000-0000-000000000001"
CARPET_INSTALLER="10000000-0000-0000-0000-000000000002"
HARDWOOD_SPECIALIST="10000000-0000-0000-0000-000000000003"

echo "🌱 Seeding demo data to Render..."

# Create Contractors
echo "Creating contractors..."

# Contractor 1: Tile Installer - Downtown
curl -s -X POST "$API_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Premium Tile Works",
    "jobTypeId": "'$TILE_INSTALLER'",
    "baseLatitude": 41.8781,
    "baseLongitude": -87.6298,
    "baseAddress": "123 Main St, Downtown Chicago",
    "phone": "555-0101",
    "email": "contact@premiumtile.com",
    "rating": 4.8
  }' | jq -r '.id' > /tmp/contractor1_id.txt

CONTRACTOR1=$(cat /tmp/contractor1_id.txt)
echo "✅ Created Premium Tile Works: $CONTRACTOR1"

# Contractor 2: Carpet Installer - North Side
curl -s -X POST "$API_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Cozy Carpet Solutions",
    "jobTypeId": "'$CARPET_INSTALLER'",
    "baseLatitude": 41.9742,
    "baseLongitude": -87.6589,
    "baseAddress": "456 Oak Ave, North Side Chicago",
    "phone": "555-0102",
    "email": "info@cozycarpet.com",
    "rating": 4.5
  }' | jq -r '.id' > /tmp/contractor2_id.txt

CONTRACTOR2=$(cat /tmp/contractor2_id.txt)
echo "✅ Created Cozy Carpet Solutions: $CONTRACTOR2"

# Contractor 3: Hardwood Specialist - South Side
curl -s -X POST "$API_URL/contractors" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Elite Hardwood Flooring",
    "jobTypeId": "'$HARDWOOD_SPECIALIST'",
    "baseLatitude": 41.7500,
    "baseLongitude": -87.6250,
    "baseAddress": "789 Pine Rd, South Side Chicago",
    "phone": "555-0103",
    "email": "service@elitehardwood.com",
    "rating": 4.9
  }' | jq -r '.id' > /tmp/contractor3_id.txt

CONTRACTOR3=$(cat /tmp/contractor3_id.txt)
echo "✅ Created Elite Hardwood Flooring: $CONTRACTOR3"

# Create Jobs
echo ""
echo "Creating jobs..."

# Calculate tomorrow's date
TOMORROW=$(date -d "+1 day" +%Y-%m-%d)

# Job 1: Tile installation - Downtown
curl -s -X POST "$API_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "jobTypeId": "'$TILE_INSTALLER'",
    "customerId": "CUST-001",
    "customerName": "Acme Corporation",
    "latitude": 41.8785,
    "longitude": -87.6295,
    "desiredDate": "'$TOMORROW'",
    "desiredTime": "10:00:00",
    "estimatedDurationHours": 4.0
  }' > /dev/null

echo "✅ Created tile installation job"

# Job 2: Carpet replacement - North Side
curl -s -X POST "$API_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "jobTypeId": "'$CARPET_INSTALLER'",
    "customerId": "CUST-002",
    "customerName": "North Plaza Apartments",
    "latitude": 41.9750,
    "longitude": -87.6600,
    "desiredDate": "'$TOMORROW'",
    "desiredTime": "14:00:00",
    "estimatedDurationHours": 6.0
  }' > /dev/null

echo "✅ Created carpet replacement job"

# Job 3: Hardwood refinishing - South Side
curl -s -X POST "$API_URL/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "jobTypeId": "'$HARDWOOD_SPECIALIST'",
    "customerId": "CUST-003",
    "customerName": "Luxury Homes LLC",
    "latitude": 41.7520,
    "longitude": -87.6240,
    "desiredDate": "'$TOMORROW'",
    "desiredTime": "09:00:00",
    "estimatedDurationHours": 8.0
  }' > /dev/null

echo "✅ Created hardwood refinishing job"

echo ""
echo "🎉 Demo data seeded successfully!"
echo "Visit https://smartscheduler-frontend-ltq9.onrender.com to see the data"
