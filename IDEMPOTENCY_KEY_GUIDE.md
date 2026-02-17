# Idempotency Key in Bookings API

## What is an Idempotency Key?

An **idempotency key** is a unique identifier that you (the client) generate and send with your API request. It ensures that if the same request is sent multiple times (due to network issues, retries, etc.), the operation will only be performed once.

## Where Does the Idempotency Key Come From?

**The client generates it** - not the server. When calling `POST /api/bookings`, you need to generate a unique string and include it in the request body.

## How to Generate an Idempotency Key

### Option 1: Use a GUID/UUID (Recommended)
```csharp
// C# example
var idempotencyKey = Guid.NewGuid().ToString();
// Example: "550e8400-e29b-41d4-a716-446655440000"
```

```javascript
// JavaScript example
const idempotencyKey = crypto.randomUUID();
// Example: "550e8400-e29b-41d4-a716-446655440000"
```

```python
# Python example
import uuid
idempotency_key = str(uuid.uuid4())
# Example: "550e8400-e29b-41d4-a716-446655440000"
```

### Option 2: Combine User ID + Slot ID + Timestamp
```csharp
var idempotencyKey = $"booking-{userId}-{slotId}-{DateTime.UtcNow.Ticks}";
// Example: "booking-123-456-638412345678901234"
```

### Option 3: Use a timestamp-based approach
```csharp
var idempotencyKey = $"booking-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
// Example: "booking-20260217135000123-a1b2c3d4"
```

## Example API Request

```http
POST /api/bookings
Authorization: Bearer <your-jwt-token>
Content-Type: application/json

{
  "slotId": 123,
  "discountCode": "SUMMER2026",
  "idempotencyKey": "550e8400-e29b-41d4-a716-446655440000"
}
```

## How It Works

1. **First Request**: When you send a booking request with an idempotency key, the system:
   - Checks if a booking with that key already exists
   - If not, creates the booking
   - Stores the idempotency key with the booking

2. **Duplicate Request**: If you send the same request again (with the same idempotency key):
   - The system finds the existing booking
   - Returns the existing booking instead of creating a duplicate
   - No error occurs - it's treated as a successful idempotent operation

## Why Is This Important?

### Without Idempotency Keys:
```
User clicks "Book" → Network timeout
User clicks "Book" again → Network timeout
User clicks "Book" again → Success
Result: 3 bookings created! 💸
```

### With Idempotency Keys:
```
User clicks "Book" (key: "abc123") → Network timeout
User clicks "Book" again (same key: "abc123") → Network timeout
User clicks "Book" again (same key: "abc123") → Success
Result: 1 booking created! ✅
```

## Complete Example Flow

### Step 1: Generate Idempotency Key
```csharp
var idempotencyKey = Guid.NewGuid().ToString();
```

### Step 2: Lock the Slot
```http
POST /api/slots/123/lock
Authorization: Bearer <token>

Response:
{
  "slotId": 123,
  "lockToken": "lock-xyz789",
  "lockedUntil": "2026-02-17T14:00:00Z"
}
```

### Step 3: Create Booking with Idempotency Key
```http
POST /api/bookings
Authorization: Bearer <token>
Content-Type: application/json

{
  "slotId": 123,
  "discountCode": null,
  "idempotencyKey": "550e8400-e29b-41d4-a716-446655440000"
}

Response:
{
  "bookingId": 456,
  "userId": 789,
  "slotId": 123,
  "status": "Pending",
  "finalPrice": 150.00,
  "createdAt": "2026-02-17T13:55:00Z"
}
```

### Step 4: Confirm Booking
```http
POST /api/bookings/456/confirm
Authorization: Bearer <token>
Content-Type: application/json

{
  "lockToken": "lock-xyz789"
}

Response:
{
  "bookingId": 456,
  "status": "Confirmed",
  "confirmedAt": "2026-02-17T13:56:00Z"
}
```

## Best Practices

1. **Generate on Client Side**: Always generate the idempotency key on the client before sending the request
2. **Store Temporarily**: Keep the key in memory or session storage while the request is pending
3. **Reuse for Retries**: Use the same key if you need to retry the same operation
4. **New Key for New Operations**: Generate a new key for each new booking attempt
5. **Don't Share Keys**: Each unique operation should have its own unique key

## Common Mistakes to Avoid

❌ **Wrong**: Using a random value on every retry
```csharp
// DON'T DO THIS
async void BookSlot() {
    var idempotencyKey = Guid.NewGuid().ToString(); // New key every time!
    await CreateBooking(slotId, idempotencyKey);
}
```

✅ **Correct**: Generate once, reuse for retries
```csharp
// DO THIS
async void BookSlot() {
    var idempotencyKey = Guid.NewGuid().ToString(); // Generate once
    
    int retries = 3;
    for (int i = 0; i < retries; i++) {
        try {
            await CreateBooking(slotId, idempotencyKey); // Same key for retries
            break;
        } catch (NetworkException) {
            if (i == retries - 1) throw;
            await Task.Delay(1000);
        }
    }
}
```

## Summary

- **Source**: You generate it (client-side)
- **Format**: Any unique string (GUID recommended)
- **Purpose**: Prevent duplicate bookings
- **When**: Include it in every `POST /api/bookings` request
- **Reuse**: Same key for retries of the same operation
- **New Key**: Different operations need different keys
