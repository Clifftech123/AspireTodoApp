import type { ReactNode } from 'react'
import { cn } from '@/lib/cn'

export interface EmptyStateProps {
  title?: string
  description?: string
  icon?: ReactNode
  action?: ReactNode
  className?: string
}

export const EmptyState = ({
  title = 'No results',
  description = 'There is nothing to show here yet.',
  icon,
  action,
  className,
}: EmptyStateProps) => (
  <div
    className={cn(
      'flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed border-slate-300 bg-white p-10 text-center',
      className,
    )}
  >
    {icon && <div className="text-slate-400">{icon}</div>}
    <h3 className="text-base font-semibold text-slate-900">{title}</h3>
    <p className="max-w-sm text-sm text-slate-500">{description}</p>
    {action}
  </div>
)