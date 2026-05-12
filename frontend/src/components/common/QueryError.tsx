import { cn } from '@/lib/cn'
import { normalizeError } from '@/api/errors'
import { Button } from './Button'

export interface QueryErrorProps {
  error: unknown
  onRetry?: () => void
  className?: string
  title?: string
}

export const QueryError = ({
  error,
  onRetry,
  className,
  title = 'Something went wrong',
}: QueryErrorProps) => {
  const normalized = normalizeError(error)
  return (
    <div
      className={cn(
        'flex flex-col items-center justify-center gap-3 rounded-lg border border-red-200 bg-red-50 p-6 text-center',
        className,
      )}
      role="alert"
    >
      <h3 className="text-base font-semibold text-red-800">{title}</h3>
      <p className="max-w-md text-sm text-red-700">{normalized.message}</p>
      {onRetry && (
        <Button variant="danger" size="sm" onClick={onRetry}>
          Try again
        </Button>
      )}
    </div>
  )
}