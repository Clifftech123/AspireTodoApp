import { Spinner } from './Spinner'

export const LoadingScreen = ({ message = 'Loading…' }: { message?: string }) => (
  <div className="flex min-h-[50vh] flex-col items-center justify-center gap-3 text-slate-500">
    <Spinner size="lg" className="text-indigo-600" />
    <p className="text-sm">{message}</p>
  </div>
)