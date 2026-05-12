import { cn } from '@/lib/cn'

type Size = 'sm' | 'md' | 'lg'

const sizes: Record<Size, string> = {
  sm: 'h-4 w-4 border-2',
  md: 'h-6 w-6 border-2',
  lg: 'h-10 w-10 border-4',
}

export interface SpinnerProps {
  size?: Size
  className?: string
  label?: string
}

export const Spinner = ({ size = 'md', className, label = 'Loading' }: SpinnerProps) => (
  <span
    role="status"
    aria-label={label}
    className={cn(
      'inline-block animate-spin rounded-full border-current border-t-transparent',
      sizes[size],
      className,
    )}
  />
)