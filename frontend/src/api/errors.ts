import axios, { type AxiosError } from 'axios'
import type { ApiErrorShape } from '@/types'

export class ApiError extends Error implements ApiErrorShape {
  status?: number
  code?: string
  details?: Record<string, unknown>

  constructor(message: string, options: Omit<ApiErrorShape, 'message'> = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = options.status
    this.code = options.code
    this.details = options.details
  }
}

interface ServerErrorBody {
  message?: string
  error?: string
  errors?: Record<string, unknown>
  code?: string
}

const pickServerMessage = (data: unknown): string | undefined => {
  if (!data || typeof data !== 'object') return undefined
  const body = data as ServerErrorBody
  if (typeof body.message === 'string') return body.message
  if (typeof body.error === 'string') return body.error
  return undefined
}

export function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) return error

  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<ServerErrorBody>
    const status = axiosError.response?.status
    const serverMessage = pickServerMessage(axiosError.response?.data)
    const fallback = status
      ? `Request failed with status ${status}`
      : axiosError.message || 'Network request failed'

    return new ApiError(serverMessage ?? fallback, {
      status,
      code: axiosError.code,
      details: axiosError.response?.data as Record<string, unknown> | undefined,
    })
  }

  if (error instanceof Error) return new ApiError(error.message)
  return new ApiError('An unknown error occurred')
}

export function normalizeError(error: unknown): ApiError {
  return toApiError(error)
}

export const isUnauthorized = (error: unknown): boolean => toApiError(error).status === 401

export const isForbidden = (error: unknown): boolean => toApiError(error).status === 403

export const isNotFound = (error: unknown): boolean => toApiError(error).status === 404
