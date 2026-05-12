export const API_CONFIG = {
  DEFAULT_TIMEOUT: 15_000,
  DEFAULT_RETRIES: 1,
  DEFAULT_STALE_TIME: 60_000,
  DEFAULT_GC_TIME: 5 * 60_000,
} as const

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'cliftech.auth.token',
  AUTH_USER: 'cliftech.auth.user',
} as const