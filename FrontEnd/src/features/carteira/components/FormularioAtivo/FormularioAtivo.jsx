import { Alert } from '../../../../components/Alert'
import { Button } from '../../../../components/Button'
import { Card } from '../../../../components/Card'
import { Input } from '../../../../components/Input'
import { Select } from '../../../../components/Select'
import { TIPOS_ATIVO } from '../../constants/tiposAtivo'

export function FormularioAtivo({
  formulario,
  atualizarCampo,
  enviando,
  erro,
  enviarFormulario,
}) {
  return (
    <Card titulo="Adicionar ativo">
      {erro && (
        <Alert className="mb-4 px-3 py-2 text-sm">{erro}</Alert>
      )}
      <form
        onSubmit={enviarFormulario}
        className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5"
      >
        <Input
          placeholder="Ticker (ex: PETR4)"
          value={formulario.ticker}
          onChange={atualizarCampo('ticker')}
          required
        />
        <Input
          type="number"
          step="0.01"
          min="0"
          placeholder="Preço médio"
          value={formulario.precoMedio}
          onChange={atualizarCampo('precoMedio')}
          required
        />
        <Input
          type="number"
          step="0.0001"
          min="0"
          placeholder="Quantidade"
          value={formulario.quantidade}
          onChange={atualizarCampo('quantidade')}
          required
        />
        <Select
          value={formulario.tipo}
          onChange={atualizarCampo('tipo')}
        >
          {TIPOS_ATIVO.map((tipo) => (
            <option key={tipo.valor} value={tipo.valor}>
              {tipo.rotulo}
            </option>
          ))}
        </Select>
        <Button type="submit" disabled={enviando}>
          {enviando ? 'Salvando...' : 'Adicionar'}
        </Button>
      </form>
    </Card>
  )
}
