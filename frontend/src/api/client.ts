import axios, { AxiosHeaders, type InternalAxiosRequestConfig } from 'axios'
import { env, isDev } from '@/config/env'
import { STORAGE_KEYS } from '@/constants/api'
import { storage } from '@/utils/storage'
import { toApiError } from './errors'

export const apiClient = axios.create({
  baseURL: env.API_URL,
  timeout: env.API_TIMEOUT,
  headers: { 'Content-Type': 'application/json' },
})

apiClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = storage.get<string>(STORAGE_KEYS.AUTH_TOKEN)
  if (token) {
    const headers = AxiosHeaders.from(config.headers)
    headers.set('Authorization', `Bearer ${token}`)
    config.headers = headers
  }
  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const apiError = toApiError(error)

    if (apiError.status === 401) {
      storage.remove(STORAGE_KEYS.AUTH_TOKEN)
      storage.remove(STORAGE_KEYS.AUTH_USER)
    }

    if (isDev) {
      console.error('[api]', apiError.status ?? '—', apiError.message, apiError.details)
    }

    return Promise.reject(apiError)
  },
)

export const setAuthToken = (token: string | null) => {
  if (token) {
    storage.set(STORAGE_KEYS.AUTH_TOKEN, token)
  } else {
    storage.remove(STORAGE_KEYS.AUTH_TOKEN)
  }
}
