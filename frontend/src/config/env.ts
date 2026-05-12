export const env = {
  API_URL: '/api',
  API_TIMEOUT: 15_000,
} as const

export const isDev = import.meta.env.DEV
