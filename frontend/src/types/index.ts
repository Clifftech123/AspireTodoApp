import type { ReactNode } from 'react'

export type WithChildren<T = object> = T & { children: ReactNode }
export type Nullable<T> = T | null
export type Maybe<T> = T | undefined
export type ValueOf<T> = T[keyof T]

export interface ApiResponse<T> {
  data: T
  message?: string
}

export interface Paginated<T> {
  data: T[]
  page: number
  pageSize: number
  total: number
}

export interface ApiErrorShape {
  message: string
  status?: number
  code?: string
  details?: Record<string, unknown>
}
