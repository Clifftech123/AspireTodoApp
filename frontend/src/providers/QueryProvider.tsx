import { useState, type ReactNode } from 'react'
import { QueryCache, QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { toast } from 'react-toastify'
import { API_CONFIG } from '@/constants/api'
import { isDev } from '@/config/env'
import { normalizeError } from '@/api/errors'

const createQueryClient = () =>
  new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: API_CONFIG.DEFAULT_STALE_TIME,
        gcTime: API_CONFIG.DEFAULT_GC_TIME,
        retry: (failureCount, error) => {
          const status = normalizeError(error).status
          if (status && status >= 400 && status < 500) return false
          return failureCount < API_CONFIG.DEFAULT_RETRIES
        },
        refetchOnWindowFocus: false,
      },
      mutations: {
        retry: 0,
        onError: (error) => {
          toast.error(normalizeError(error).message)
        },
      },
    },
    queryCache: new QueryCache({
      onError: (error, query) => {
        if (query.state.data === undefined) {
          toast.error(normalizeError(error).message)
        }
      },
    }),
  })

export const QueryProvider = ({ children }: { children: ReactNode }) => {
  const [client] = useState(createQueryClient)
  return (
    <QueryClientProvider client={client}>
      {children}
      {isDev && <ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-right" />}
    </QueryClientProvider>
  )
}