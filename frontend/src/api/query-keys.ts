export const queryKeys = {
  posts: {
    all: ['posts'] as const,
    lists: () => [...queryKeys.posts.all, 'list'] as const,
    list: (filters?: Record<string, unknown>) =>
      [...queryKeys.posts.lists(), filters ?? {}] as const,
    details: () => [...queryKeys.posts.all, 'detail'] as const,
    detail: (id: number | string) => [...queryKeys.posts.details(), id] as const,
  },
  users: {
    all: ['users'] as const,
    me: () => [...queryKeys.users.all, 'me'] as const,
    detail: (id: number | string) => [...queryKeys.users.all, 'detail', id] as const,
  },
} as const
