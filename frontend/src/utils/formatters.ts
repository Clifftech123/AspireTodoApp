export const formatDate = (
  value: Date | string | number,
  locale = 'en-US',
  options: Intl.DateTimeFormatOptions = { year: 'numeric', month: 'short', day: 'numeric' },
): string => new Intl.DateTimeFormat(locale, options).format(new Date(value))

export const formatDateTime = (value: Date | string | number, locale = 'en-US'): string =>
  new Intl.DateTimeFormat(locale, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))

export const formatCurrency = (value: number, currency = 'USD', locale = 'en-US'): string =>
  new Intl.NumberFormat(locale, { style: 'currency', currency }).format(value)

export const formatNumber = (value: number, locale = 'en-US'): string =>
  new Intl.NumberFormat(locale).format(value)

export const truncate = (text: string, max = 80): string =>
  text.length > max ? `${text.slice(0, max).trimEnd()}…` : text

export const capitalize = (text: string): string =>
  text ? text.charAt(0).toUpperCase() + text.slice(1) : text

export const slugify = (text: string): string =>
  text
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')