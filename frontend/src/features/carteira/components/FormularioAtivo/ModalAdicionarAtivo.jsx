import { Modal } from '../../../../components/Modal'
import { FormularioAtivo } from './FormularioAtivo'

export function ModalAdicionarAtivo({ aberto, onFechar, formularioAtivo }) {
  return (
    <Modal aberto={aberto} onFechar={onFechar} titulo="Adicionar ativo" tamanho="lg">
      <FormularioAtivo {...formularioAtivo} />
    </Modal>
  )
}
