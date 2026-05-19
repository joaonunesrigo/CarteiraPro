import { useAdicionarAtivo, useExcluirAtivo } from '../../features/carteira/hooks/useAtivos'
import { useCarteira } from '../../features/carteira/hooks/useCarteira'
import { Dashboard } from './Dashboard'

export default function DashboardPage() {
  const { cartoesResumo, linhasAtivos, carregando, erro, recarregar } =
    useCarteira()

  const formularioAtivo = useAdicionarAtivo(recarregar)
  const { excluirAtivo } = useExcluirAtivo(recarregar)

  return (
    <Dashboard
      cartoesResumo={cartoesResumo}
      linhasAtivos={linhasAtivos}
      carregando={carregando}
      erro={erro}
      formularioAtivo={formularioAtivo}
      excluirAtivo={excluirAtivo}
    />
  )
}
