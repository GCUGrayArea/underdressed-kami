/**
 * Form Validation Utilities
 *
 * Client-side validation helpers matching backend validation rules.
 * These rules mirror the FluentValidation validators in the backend.
 */

/**
 * Validates contractor name length (2-100 characters)
 */
export function validateContractorName(name: string): string | undefined {
  if (!name || name.trim().length === 0) {
    return 'Name is required';
  }
  if (name.length < 2) {
    return 'Name must be at least 2 characters';
  }
  if (name.length > 100) {
    return 'Name must not exceed 100 characters';
  }
  return undefined;
}

/**
 * Validates contractor rating (0.0-5.0)
 */
export function validateRating(rating: number | undefined): string | undefined {
  if (rating === undefined || rating === null) {
    return undefined; // Rating is optional, defaults to 3.0 on backend
  }
  if (rating < 0 || rating > 5) {
    return 'Rating must be between 0 and 5';
  }
  return undefined;
}

/**
 * Validates latitude (-90 to 90)
 */
export function validateLatitude(lat: number | undefined): string | undefined {
  if (lat === undefined || lat === null) {
    return 'Latitude is required';
  }
  if (lat < -90 || lat > 90) {
    return 'Latitude must be between -90 and 90';
  }
  return undefined;
}

/**
 * Validates longitude (-180 to 180)
 */
export function validateLongitude(lon: number | undefined): string | undefined {
  if (lon === undefined || lon === null) {
    return 'Longitude is required';
  }
  if (lon < -180 || lon > 180) {
    return 'Longitude must be between -180 and 180';
  }
  return undefined;
}

/**
 * Validates email format (basic validation)
 */
export function validateEmail(email: string | undefined): string | undefined {
  if (!email || email.trim().length === 0) {
    return undefined; // Email is optional
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    return 'Invalid email format';
  }

  return undefined;
}

/**
 * Validates phone number (basic validation, allows various formats)
 */
export function validatePhone(phone: string | undefined): string | undefined {
  if (!phone || phone.trim().length === 0) {
    return undefined; // Phone is optional
  }

  // Allow digits, spaces, parentheses, dashes, plus sign
  const phoneRegex = /^[\d\s()+-]+$/;
  if (!phoneRegex.test(phone)) {
    return 'Phone number can only contain digits, spaces, parentheses, dashes, and plus sign';
  }

  // Ensure at least 10 digits
  const digitsOnly = phone.replace(/\D/g, '');
  if (digitsOnly.length < 10) {
    return 'Phone number must contain at least 10 digits';
  }

  return undefined;
}

/**
 * Validates time format (HH:mm)
 */
export function validateTimeFormat(time: string | undefined): string | undefined {
  if (!time || time.trim().length === 0) {
    return 'Time is required';
  }

  const timeRegex = /^([0-1]\d|2[0-3]):([0-5]\d)$/;
  if (!timeRegex.test(time)) {
    return 'Time must be in HH:mm format (00:00 - 23:59)';
  }

  return undefined;
}

/**
 * Validates that end time is after start time
 */
export function validateTimeRange(startTime: string, endTime: string): string | undefined {
  const startMinutes = timeToMinutes(startTime);
  const endMinutes = timeToMinutes(endTime);

  if (endMinutes <= startMinutes) {
    return 'End time must be after start time';
  }

  return undefined;
}

/**
 * Helper to convert time string (HH:mm) to minutes since midnight
 */
function timeToMinutes(time: string): number {
  const [hours, minutes] = time.split(':').map(Number);
  return hours * 60 + minutes;
}

/**
 * Validates that at least one working hour slot is defined
 */
export function validateWorkingHours(
  schedules: Array<{ dayOfWeek: number; startTime: string; endTime: string; enabled: boolean }>
): string | undefined {
  const hasAnyEnabledSlot = schedules.some(schedule => schedule.enabled);

  if (!hasAnyEnabledSlot) {
    return 'At least one working day must be defined';
  }

  return undefined;
}

/**
 * Validates customer ID (1-100 characters)
 */
export function validateCustomerId(customerId: string): string | undefined {
  if (!customerId || customerId.trim().length === 0) {
    return 'Customer ID is required';
  }
  if (customerId.length > 100) {
    return 'Customer ID must not exceed 100 characters';
  }
  return undefined;
}

/**
 * Validates customer name (1-200 characters)
 */
export function validateCustomerName(customerName: string): string | undefined {
  if (!customerName || customerName.trim().length === 0) {
    return 'Customer name is required';
  }
  if (customerName.length > 200) {
    return 'Customer name must not exceed 200 characters';
  }
  return undefined;
}

/**
 * Validates estimated duration (0.1-24 hours)
 */
export function validateDuration(duration: number | undefined): string | undefined {
  if (duration === undefined || duration === null) {
    return 'Estimated duration is required';
  }
  if (duration < 0.1 || duration > 24) {
    return 'Duration must be between 0.1 and 24 hours';
  }
  return undefined;
}

/**
 * Validates date is in the future
 */
export function validateFutureDate(date: Date | null): string | undefined {
  if (!date) {
    return 'Desired date is required';
  }

  // Set time to start of day for comparison
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const selectedDate = new Date(date);
  selectedDate.setHours(0, 0, 0, 0);

  if (selectedDate < today) {
    return 'Desired date must be today or in the future';
  }

  return undefined;
}

/**
 * Validates address (optional, max 500 characters)
 */
export function validateAddress(address: string | undefined): string | undefined {
  if (!address || address.trim().length === 0) {
    return undefined; // Address is optional
  }
  if (address.length > 500) {
    return 'Address must not exceed 500 characters';
  }
  return undefined;
}
