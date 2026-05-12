import { z } from 'zod'
import { http } from './http'

export interface ResourceConfig<TSchema extends z.ZodTypeAny, TListSchema extends z.ZodTypeAny> {
  baseUrl: string
  schema: TSchema
  listSchema?: TListSchema
}

export function createResource<
  TSchema extends z.ZodTypeAny,
  TListSchema extends z.ZodTypeAny = z.ZodArray<TSchema>,
>(config: ResourceConfig<TSchema, TListSchema>) {
  const listSchema = (config.listSchema ?? z.array(config.schema)) as TListSchema

  type TItem = z.infer<TSchema>
  type TList = z.infer<TListSchema>

  return {
    list: (params?: Record<string, unknown>): Promise<TList> =>
      http.get<TList, TListSchema>(config.baseUrl, { params, schema: listSchema }),

    getById: (id: number | string): Promise<TItem> =>
      http.getById<TItem, TSchema>(config.baseUrl, id, { schema: config.schema }),

    create: <TBody>(body: TBody): Promise<TItem> =>
      http.post<TItem, TSchema>(config.baseUrl, body, { schema: config.schema }),

    update: <TBody>(id: number | string, body: TBody): Promise<TItem> =>
      http.patch<TItem, TSchema>(`${config.baseUrl}/${id}`, body, { schema: config.schema }),

    replace: <TBody>(id: number | string, body: TBody): Promise<TItem> =>
      http.put<TItem, TSchema>(`${config.baseUrl}/${id}`, body, { schema: config.schema }),

    remove: (id: number | string): Promise<void> => http.delete<void>(`${config.baseUrl}/${id}`),
  }
}
