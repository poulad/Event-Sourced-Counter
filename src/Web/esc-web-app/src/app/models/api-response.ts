export interface ApiResponse<T> {
  ok: boolean;
  value?: T;
  message?: string;
  correlationId?: string;
}
