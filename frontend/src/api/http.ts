import type { AxiosRequestConfig } from 'axios'
import type { z } from 'zod'
import { apiClient } from './client'

type Params = Record<string, unknown> | undefined

interface RequestOptions<TSchema extends z.ZodTypeAny | undefined = undefined> extends Omit<
  AxiosRequestConfig,
  'url' | 'method' | 'params' | 'data' | 'baseURL'
> {
  params?: Params
  schema?: TSchema
}

type Parsed<TSchema, TFallback> = TSchema extends z.ZodTypeAny ? z.infer<TSchema> : TFallback

const unwrap = <TSchema extends z.ZodTypeAny | undefined, T>(
  data: unknown,
  schema?: TSchema,
): Parsed<TSchema, T> => (schema ? schema.parse(data) : (data as T)) as Parsed<TSchema, T>

export const http = {
  async get<T = unknown, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    const { schema, params, ...rest } = options
    const { data } = await apiClient.get(url, { ...rest, params })
    return unwrap<TSchema, T>(data, schema)
  },

  async getById<T = unknown, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    id: number | string,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    return http.get<T, TSchema>(`${url}/${id}`, options)
  },

  async post<T = unknown, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    body?: unknown,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    const { schema, params, ...rest } = options
    const { data } = await apiClient.post(url, body, { ...rest, params })
    return unwrap<TSchema, T>(data, schema)
  },

  async put<T = unknown, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    body?: unknown,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    const { schema, params, ...rest } = options
    const { data } = await apiClient.put(url, body, { ...rest, params })
    return unwrap<TSchema, T>(data, schema)
  },

  async patch<T = unknown, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    body?: unknown,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    const { schema, params, ...rest } = options
    const { data } = await apiClient.patch(url, body, { ...rest, params })
    return unwrap<TSchema, T>(data, schema)
  },

  async delete<T = void, TSchema extends z.ZodTypeAny | undefined = undefined>(
    url: string,
    options: RequestOptions<TSchema> = {},
  ): Promise<Parsed<TSchema, T>> {
    const { schema, params, ...rest } = options
    const { data } = await apiClient.delete(url, { ...rest, params })
    return unwrap<TSchema, T>(data, schema)
  },
}
