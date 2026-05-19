import { Alert } from '../../components/Alert'
import { Tabs } from '../../components/Tabs'
import { AtivoTable } from '../../features/carteira/components/AtivoTable'
import { CarteiraGraficos } from '../../features/carteira/components/CarteiraGraficos'
import { CarteiraSummary } from '../../features/carteira/components/CarteiraSummary'
import { FormularioAtivo } from '../../features/carteira/components/FormularioAtivo'

export function Dashboard({
  abas,
  abaAtiva,
  onMudarAba,
  cartoesResumo,
  linhasAtivos,
  dadosGraficos,
  carregando,
  erro,
  formularioAtivo,
  excluirAtivo,
}) {
  const paineis = {
    carteira: (
      <>
        <CarteiraSummary cartoes={cartoesResumo} />
        <FormularioAtivo {...formularioAtivo} />
        <AtivoTable linhas={linhasAtivos} excluirAtivo={excluirAtivo} />
      </>
    ),
    graficos: <CarteiraGraficos dadosGraficos={dadosGraficos} />,
  }

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 px-6 py-8">
        <h1 className="text-2xl font-semibold tracking-tight text-white">
          CarteiraPro
        </h1>
        <p className="mt-1 text-slate-400">Dashboard da sua carteira</p>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        {erro && (
          <Alert className="mb-8">
            {erro}
            <span className="mt-1 block text-sm text-red-400/80">
              Verifique se a API está rodando em http://localhost:5122
            </span>
          </Alert>
        )}

        {carregando ? (
          <p className="text-slate-400">Carregando...</p>
        ) : (
          <Tabs
            abas={abas}
            abaAtiva={abaAtiva}
            onMudarAba={onMudarAba}
            paineis={paineis}
          />
        )}
      </main>
    </div>
  )
}
