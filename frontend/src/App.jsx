import { QueryClientProvider } from '@tanstack/react-query'
import { ConfirmDialogProvider } from './components/ConfirmDialog'
import { ToastProvider } from './components/Toast'
import { AuthPage } from './features/auth/components/AuthPage'
import { useAuth } from './features/auth/hooks/useAuth'
import { queryClient } from './lib/queryClient'
import Dashboard from './pages/Dashboard'

function AppContent() {
  const auth = useAuth()

  function logout() {
    auth.logout()
    queryClient.clear()
  }

  if (auth.carregandoSessao) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-slate-950 text-slate-300">
        Carregando sessão...
      </main>
    )
  }

  if (!auth.autenticado) {
    return <AuthPage auth={auth} />
  }

  return <Dashboard usuario={auth.usuario} logout={logout} />
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ConfirmDialogProvider>
        <ToastProvider>
          <AppContent />
        </ToastProvider>
      </ConfirmDialogProvider>
    </QueryClientProvider>
  )
}
