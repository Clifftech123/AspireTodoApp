export const ROUTES = {
  HOME: '/',
  TODOS: '/todos',
  NOT_FOUND: '*',
} as const

export type AppRoute = (typeof ROUTES)[keyof typeof ROUTES]
