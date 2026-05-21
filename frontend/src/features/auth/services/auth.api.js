import { apiClient } from '../../carteira/services/apiClient'

export const authApi = {
  registrar: (dados) =>
    apiClient('/auth/registrar', {
      method: 'POST',
      body: JSON.stringify(dados),
      auth: false,
    }),
  login: (dados) =>
    apiClient('/auth/login', {
      method: 'POST',
      body: JSON.stringify(dados),
      auth: false,
    }),
  me: () => apiClient('/auth/me'),
}
