export const ENDPOINTS = {
  posts: {
    list: '/posts',
    detail: (id: number | string) => `/posts/${id}`,
  },
  users: {
    list: '/users',
    detail: (id: number | string) => `/users/${id}`,
    me: '/users/me',
  },
  auth: {
    login: '/auth/login',
    register: '/auth/register',
    logout: '/auth/logout',
    refresh: '/auth/refresh',
  },
} as const
