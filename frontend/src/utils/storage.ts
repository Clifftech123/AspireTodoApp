export const storage = {
  get<T>(key: string): T | null {
    try {
      const raw = window.localStorage.getItem(key)
      return raw ? (JSON.parse(raw) as T) : null
    } catch {
      return null
    }
  },
  set<T>(key: string, value: T): void {
    try {
      window.localStorage.setItem(key, JSON.stringify(value))
    } catch (error) {
      console.error(`Failed to write "${key}" to localStorage`, error)
    }
  },
  remove(key: string): void {
    try {
      window.localStorage.removeItem(key)
    } catch (error) {
      console.error(`Failed to remove "${key}" from localStorage`, error)
    }
  },
  clear(): void {
    try {
      window.localStorage.clear()
    } catch (error) {
      console.error('Failed to clear localStorage', error)
    }
  },
}
