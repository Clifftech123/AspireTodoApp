import { useCallback, useState } from 'react'

export function useToggle(initial = false) {
  const [value, setValue] = useState(initial)
  const toggle = useCallback(() => setValue((prev) => !prev), [])
  const on = useCallback(() => setValue(true), [])
  const off = useCallback(() => setValue(false), [])
  return { value, toggle, on, off, setValue } as const
}