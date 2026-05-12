import type { ReactNode } from 'react'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/ReactToastify.css'
import { ErrorBoundary } from '@/components/common'
import { QueryProvider } from './QueryProvider'

export const AppProviders = ({ children }: { children: ReactNode }) => (
  <ErrorBoundary>
    <QueryProvider>
      {children}
      <ToastContainer position="top-right" autoClose={3000} newestOnTop />
    </QueryProvider>
  </ErrorBoundary>
)
