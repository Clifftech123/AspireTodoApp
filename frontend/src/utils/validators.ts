import { z } from 'zod'

export const emailSchema = z.string().trim().email('Enter a valid email address')

export const passwordSchema = z
  .string()
  .min(8, 'Password must be at least 8 characters')
  .regex(/[A-Z]/, 'Include at least one uppercase letter')
  .regex(/[0-9]/, 'Include at least one number')

export const phoneSchema = z
  .string()
  .trim()
  .regex(/^\+?[0-9\s-]{7,15}$/, 'Enter a valid phone number')

export const requiredString = (label = 'This field') =>
  z.string().trim().min(1, `${label} is required`)
